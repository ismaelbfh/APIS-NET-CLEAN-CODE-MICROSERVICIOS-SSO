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
    public class ServiceMasterTiposEnvasado : Repository<TiposEnvasado>, IServiceMasterTiposEnvasado
    {
        private readonly IMapper _mapper;
        private readonly BDGestionEtiquetasContext _dbContext;
        public ServiceMasterTiposEnvasado(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> CheckTipoEnvasadoEnUso(int idTipoEnvasado)
        {
            return await _dbContext.Etiqueta.AnyAsync(e => 
                e.FK_IdEnvasado == idTipoEnvasado &&
                (e.IsDeleted == false || e.IsDeleted == null)
            );
        }

        public async Task<PaginatedResult<MasterDataDTO>> GetTiposEnvasadoList(int pageNumber, int pageSize, string filtro)
        {
            var query = this.Queryable.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro) && !string.IsNullOrEmpty(filtro))
                query = query.Where(c => c.Descripcion.ToLower().Contains(filtro.ToLower()));

            query = query.OrderBy(p => p.Descripcion);

            var totalRows = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (items == null || !items.Any())
            {
                Log.Error("No se encuentran tipos de envasado");
            }

            Log.Information("OK, se han encontrado {NumTiposEnvasado} tipos de envasado en el endpoint {Endpoint}", items.Count(), "GetTiposEnvasadoList");

            return new PaginatedResult<MasterDataDTO>
            {
                Items = _mapper.Map<List<MasterDataDTO>>(items),
                TotalRows = totalRows,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<TiposEnvasado> DeleteTipoEnvasadoById(int id)
        {
            // Obtiene la entidad desde el DbContext (está siendo rastreada)
            var entidad = await _dbContext.TiposEnvasado.FirstOrDefaultAsync(x => x.PK_IdEnvasado == id);

            if (entidad == null)
            {
                Log.Error("Error al borrar un tipo de envasado porque el id {IdEnv} no existe.", id);
                throw new KeyNotFoundException($"No se encontró el Tipo de Envasado con id {id}.");
            }
            
            // Remueve la entidad del contexto y guarda los cambios
            _dbContext.TiposEnvasado.Remove(entidad);
            try
            {
                // Paso 4: Guardar cambios y capturar errores
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para borrar un tipo de envasado: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para borrar un tipo de envasado: {inner}", ex);
            }
            Log.Information("Se ha eliminado el tipo de envasado con id {IdEnv} con éxito.", id);
            return entidad;
        }
    }
}
