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
    public class ServiceMasterIdiomasLabel : Repository<IdiomasLabels>, IServiceMasterIdiomasLabel
    {
        private readonly IMapper _mapper;
        private readonly BDGestionEtiquetasContext _dbContext;
        public ServiceMasterIdiomasLabel(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext) 
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<PaginatedResult<MasterDataDTO>> GetIdiomasLabelsList(int pageNumber, int pageSize, string filtro)
        {
            var query = this.Queryable.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro) && !string.IsNullOrEmpty(filtro))
                query = query.Where(c => c.Descripcion.ToLower().Contains(filtro.ToLower()));

            query = query.OrderBy(p => p.Descripcion);

            var totalRows = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (items == null || !items.Any())
            {
                Log.Error("No se encuentran idiomas de labels");
            }

            Log.Information("OK, se han encontrado {NumTiposCampos} idiomas de labels en el endpoint {Endpoint}", items.Count(), "GetTiposLabelsList");

            return new PaginatedResult<MasterDataDTO>
            {
                Items = _mapper.Map<List<MasterDataDTO>>(items),
                TotalRows = totalRows,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<bool> ExisteIdiomaEnLabel(int idIdiomaLabel)
        {
            var existe = await _dbContext.LabelsMaster.AnyAsync(pc => pc.FK_IdIdioma == idIdiomaLabel);
            return existe;
        }

        public async Task<IdiomasLabels> DeleteIdiomaLabelById(int id)
        {
            // Obtiene la entidad desde el DbContext (está siendo rastreada)
            var entidad = await _dbContext.IdiomasLabels.FirstOrDefaultAsync(x => x.PK_IdIdioma == id);

            if (entidad == null)
            {
                Log.Error("Error al borrar un idioma de label porque el id {IdIdiomaLabel} no existe.", id);
                throw new KeyNotFoundException($"No se encontró el idioma de label con id {id}.");
            }

            // Remueve la entidad del contexto y guarda los cambios
            _dbContext.IdiomasLabels.Remove(entidad);
            try
            {
                // Paso 4: Guardar cambios y capturar errores
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para borrar un idioma de label: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para borrar un idioma de label: {inner}", ex);
            }
            Log.Information("Se ha eliminado la idioma de label con id {IdIdiomaLabel} con éxito.", id);
            return entidad;
        }
    }
}
