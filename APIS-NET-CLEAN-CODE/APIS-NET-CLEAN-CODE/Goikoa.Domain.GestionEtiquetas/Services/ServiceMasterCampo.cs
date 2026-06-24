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
    public class ServiceMasterCampo : Repository<Campo>, IServiceMasterCampo
    {
        private readonly IMapper _mapper;
        private readonly BDGestionEtiquetasContext _dbContext;
        public ServiceMasterCampo(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext) 
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> ExisteEnPlantilla(int idCampo)
        {
             return await _dbContext.PlantillaCampos
                        .Where(pc => pc.FK_IdCampo == idCampo)
                        .AnyAsync(pc => pc.FK_IdPlantillaNavigation.IsDeleted == false || pc.FK_IdPlantillaNavigation.IsDeleted == null);
        }

        public async Task<PaginatedResult<CampoDTO>> GetCamposList(int pageNumber, int pageSize, string filtro)
        {
            var query = this.Queryable.Include(x => x.FK_IdTipoCampoNavigation).Include(x => x.FK_IdCampoNavisionNavigation).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro) && !string.IsNullOrEmpty(filtro))
            {
                string texto = filtro.Trim().ToLower();

                var tiposCamposPosibles = await _dbContext.TiposCampo.Select(x => x.Descripcion.ToLower()).ToListAsync();

                if (tiposCamposPosibles.Contains(texto))
                {
                    query = query.Where(h => h.FK_IdTipoCampoNavigation.Descripcion.ToLower() == texto);
                }
                else
                {
                    query = query.Where(h =>
                        h.NombreCampo != null && h.NombreCampo.ToLower().Contains(texto)
                    );
                }
            }
            query = query.OrderBy(p => p.NombreCampo);

            var totalRows = await query.CountAsync();
            var lLstCampos = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (lLstCampos == null || !lLstCampos.Any())
            {
                Log.Error("No se encuentran campos");
            }

            Log.Information("OK, se han encontrado {NumCampos} campos en el endpoint {Endpoint}", lLstCampos.Count(), "GetCamposList");

            var result = new PaginatedResult<CampoDTO>
            {
                Items = _mapper.Map<List<CampoDTO>>(lLstCampos),
                TotalRows = totalRows,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return result;
        }

        public async Task<Campo> DeleteCampoById(int id)
        {
            var campo = await _dbContext.Campo.FirstOrDefaultAsync(x => x.PK_IdCampo == id);
            if (campo == null)
                throw new KeyNotFoundException($"No se encontró el campo con id {id}.");

            // 1) Bloqueo si está en etiquetas activas
            bool enActivas = await _dbContext.EtiquetaCamposValores
                .AnyAsync(ecv => ecv.FK_IdCampo == id && ecv.FK_IdEtiquetaNavigation.IsDeleted == false);
            if (enActivas)
                throw new InvalidOperationException("No se puede borrar este campo porque está en uso en una etiqueta activa.");

            // 2) Desvincula y guarda snapshot en etiquetas NO activas
            var nombre = campo.NombreCampo ?? string.Empty;

            await _dbContext.EtiquetaCamposValores
                .Where(ecv => ecv.FK_IdCampo == id && ecv.FK_IdEtiquetaNavigation.IsDeleted == true)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(ecv => ecv.FK_IdCampo, (int?)null)
                    .SetProperty(ecv => ecv.NombreCampoDespuesDeBorrado, nombre)
                );

            // 3) Borra el campo
            _dbContext.Campo.Remove(campo);
            
            try
            {
                await _dbContext.SaveChangesAsync();
                Log.Information("Campo con id {IdCampoMaster} eliminado correctamente.", id);
                return campo;
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para borrar un campo: {inner} , excepción completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para borrar un campo: {inner}", ex);
            }
        }


    }
}
