using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Goikoa.CommonServices.Patterns;
using Goikoa.Domain.GestionEtiquetas.DAL.Context;
using Goikoa.Domain.GestionEtiquetas.DAL.Models;
using Goikoa.Domain.GestionEtiquetas.DTOs.Responses;
using Goikoa.Domain.GestionEtiquetas.Entities;
using Goikoa.Domain.GestionEtiquetas.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Goikoa.Domain.GestionEtiquetas.Services
{
    public class ServiceMasterLabelsMaster : Repository<LabelsMaster>, IServiceMasterLabels
    {
        private readonly IMapper _mapper;
        private readonly BDGestionEtiquetasContext _dbContext;

        public ServiceMasterLabelsMaster(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<bool> ExisteLabelEnEtiquetas(int idLabel)
        {
            return await _dbContext.EtiquetaLabels.AnyAsync(pc => pc.FK_IdLabel == idLabel && pc.FK_IdEtiquetaNavigation.IsDeleted == false);
        }

        /// <summary>
        /// Comprueba si ya existe una label con la misma combinación de tipo de label + idioma.
        /// Si existe, devuelve información descriptiva para mostrar en una modal.
        /// Si no existe, devuelve un DTO indicando que no hay duplicado.
        /// </summary>
        /// <param name="idTipoLabel">ID del tipo de label que se quiere crear (ej. 'OvaloCE')</param>
        /// <param name="idIdioma">ID del idioma que se quiere asignar (ej. 'Español')</param>
        /// <returns>DTO con información del posible duplicado</returns>
        public async Task<LabelDuplicadaDTO> GetInfoLabelDuplicadaAsync(int idTipoLabel, int idIdioma)
        {
            // Buscamos si ya existe una label en la tabla LabelsMaster con ese tipo e idioma
            // Incluimos navegación a sus descripciones para poder devolver texto legible
            var labelExistente = await _dbContext.LabelsMaster
                .Include(l => l.FK_IdTipoLabelNavigation)
                .Include(l => l.FK_IdIdiomaNavigation)
                .FirstOrDefaultAsync(l => l.FK_IdTipoLabel == idTipoLabel && l.FK_IdIdioma == idIdioma);

            // Si no existe, devolvemos un DTO que indica que no hay duplicado
            if (labelExistente == null)
                return new LabelDuplicadaDTO { Existe = false };

            // Si existe, devolvemos el DTO con descripciones de tipo e idioma para mostrar en el frontend
            return new LabelDuplicadaDTO
            {
                Existe = true,
                Tipo = labelExistente.FK_IdTipoLabelNavigation?.Descripcion ?? "Tipo desconocido",
                Idioma = labelExistente.FK_IdIdiomaNavigation?.Descripcion ?? "Idioma desconocido"
            };
        }

        public async Task<LabelsMaster> DeleteLabelById(int id)
        {
            var label = await _dbContext.LabelsMaster.FirstOrDefaultAsync(x => x.PK_IdLabel == id);
            if (label == null)
                throw new KeyNotFoundException($"No se encontró la label con id {id}.");

            // Verifica si está en uso en etiquetas activas (y lo bloqueas como ya haces en frontend)
            bool enUsoEnEtiquetaActiva = await _dbContext.EtiquetaLabels
                .AnyAsync(el => el.FK_IdLabel == id && el.FK_IdEtiquetaNavigation.IsDeleted == false);

            if (enUsoEnEtiquetaActiva)
                throw new InvalidOperationException("No se puede eliminar esta label porque está en uso en una etiqueta activa.");

            // Si está en etiquetas inactivas, borramos las referencias antes de eliminar
            var referenciasInactivas = await _dbContext.EtiquetaLabels
                .Where(el => el.FK_IdLabel == id && el.FK_IdEtiquetaNavigation.IsDeleted == true)
                .ToListAsync();

            _dbContext.EtiquetaLabels.RemoveRange(referenciasInactivas);
            _dbContext.LabelsMaster.Remove(label);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para borrar un label: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para borrar un label: {inner}", ex);
            }

            Log.Information("Se ha eliminado un label con id {IdLabel} con éxito.", id);
            return label;
        }

        public async Task<PaginatedResult<LabelDTO>> GetLabelsList(int pageNumber, int pageSize, string filtro)
        {

            var query = this.Queryable.Include(x => x.FK_IdIdiomaNavigation)
                                      .Include(y => y.FK_IdTipoLabelNavigation).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro) && !string.IsNullOrEmpty(filtro))
            {
                string texto = filtro.Trim().ToLower();

                // Trae lista de idiomas y tipos (o pásala al método como parámetro si ya la tienes cacheada)
                var idiomasPosibles = await _dbContext.IdiomasLabels.Select(x => x.Descripcion.ToLower()).ToListAsync();
                var tiposPosibles = await _dbContext.TiposLabels.Select(x => x.Descripcion.ToLower()).ToListAsync();

                // Prioridad: primero equals en idioma y tipo, si no, contains en descripcion
                if (idiomasPosibles.Contains(texto))
                {
                    query = query.Where(h => h.FK_IdIdiomaNavigation.Descripcion.ToLower() == texto);
                }
                else if (tiposPosibles.Contains(texto))
                {
                    query = query.Where(h => h.FK_IdTipoLabelNavigation.Descripcion.ToLower() == texto);
                }
                else
                {
                    // Contiene solo en la descripción
                    query = query.Where(h =>
                        h.DescripcionLabel != null && h.DescripcionLabel.ToLower().Contains(texto)
                    );
                }
            }
            query = query.OrderBy(p => p.DescripcionLabel);
            var totalRows = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (items == null || !items.Any())
            {
                Log.Error("No se encuentran labels");
            }

            Log.Information("OK, se han encontrado {Num} labels en el endpoint {Endpoint}", items.Count(), "GetLabelsList");

            return new PaginatedResult<LabelDTO>
            {
                Items = _mapper.Map<List<LabelDTO>>(items),
                TotalRows = totalRows,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
