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
    public class ServiceMasterTiposCampo : Repository<TiposCampo>, IServiceMasterTiposCampo
    {
        private readonly IMapper _mapper;
        private readonly BDGestionEtiquetasContext _dbContext;
        public ServiceMasterTiposCampo(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext) 
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<PaginatedResult<MasterDataDTO>> GetTiposCamposList(int pageNumber, int pageSize, string filtro)
        {
            var query = this.Queryable.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro) && !string.IsNullOrEmpty(filtro))
                query = query.Where(c => c.Descripcion.ToLower().Contains(filtro.ToLower()));

            query = query.OrderBy(p => p.Descripcion);

            var totalRows = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (items == null || !items.Any())
            {
                Log.Error("No se encuentran tipos de campos");
            }

            Log.Information("OK, se han encontrado {NumTiposCampos} tipos de campos en el endpoint {Endpoint}", items.Count(), "GetTiposCamposList");

            return new PaginatedResult<MasterDataDTO>
            {
                Items = _mapper.Map<List<MasterDataDTO>>(items),
                TotalRows = totalRows,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<bool> ExisteEnCampo(int idTipoCampo)
        {
            var existe = await _dbContext.Campo.AnyAsync(pc => pc.FK_IdTipoCampo == idTipoCampo);
            return existe;
        }

        public async Task<TiposCampo> DeleteTipoCampoById(int id)
        {
            // Obtiene la entidad desde el DbContext (está siendo rastreada)
            var entidad = await _dbContext.TiposCampo.FirstOrDefaultAsync(x => x.PK_IdTipoCampo == id);

            if (entidad == null)
            {
                Log.Error("Error al borrar un tipo de campo porque el id {IdTipoCampo} no existe.", id);
                throw new KeyNotFoundException($"No se encontró el Tipo de Campo con id {id}.");
            }

            // Remueve la entidad del contexto y guarda los cambios
            _dbContext.TiposCampo.Remove(entidad);
            try
            {
                // Paso 4: Guardar cambios y capturar errores
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para borrar un tipo de campo: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para borrar un tipo de campo: {inner}", ex);
            }
            Log.Information("Se ha eliminado el tipo de campo con id {IdTipoCampo} con éxito.", id);
            return entidad;
        }
    }
}
