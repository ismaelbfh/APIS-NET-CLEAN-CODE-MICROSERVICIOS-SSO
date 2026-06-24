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
    public class ServiceMasterTiposCodigoBarra : Repository<TiposCodigoBarra>, IServiceMasterTiposCodigoBarra
    {
        private readonly IMapper _mapper;
        private readonly BDGestionEtiquetasContext _dbContext;

        public ServiceMasterTiposCodigoBarra(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<List<CodificacionTiposCodigoBarrasDTO>> GetSelectDropdownCodificacionAsync()
        {
            return await _dbContext.CodificacionTiposCodigoBarras
            .OrderBy(x => x.Id)
            .Select(x => new CodificacionTiposCodigoBarrasDTO
            {
                Id = x.Id,
                NumeroIdentificacion = x.NumeroIdentificacion,
                NombrePropiedad = x.NombrePropiedad
            }).ToListAsync();
        }

        public async Task<bool> ExisteTipoCodigoBarraEnEtiquetas(int idTipoCodigoBarra)
        {
            return await _dbContext.EtiquetaCodigosBarra
                .AnyAsync(ecb => ecb.FK_IdTipoCodigoBarra == idTipoCodigoBarra);
        }

        public async Task<TiposCodigoBarra> DeleteTipoCodigoBarraById(int id)
        {
            // Obtiene la entidad desde el DbContext (está siendo rastreada)
            var entidad = await _dbContext.TiposCodigoBarra.FirstOrDefaultAsync(x => x.PK_IdTipoCodigoBarra == id);

            if (entidad == null)
            {
                Log.Error("Error al borrar un tipo de codigo de barras porque el id {IdCodBarras} no existe.", id);
                throw new KeyNotFoundException($"No se encontró el Tipo de Codigo de Barras con id {id}.");
            }

            // Remueve la entidad del contexto y guarda los cambios
            _dbContext.TiposCodigoBarra.Remove(entidad);
            try
            {
                // Paso 4: Guardar cambios y capturar errores
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para borrar un tipo de codigo de barras: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para borrar un tipo de codigo de barras: {inner}", ex);
            }
            Log.Information("Se ha eliminado el tipo de codigo de barras con id {IdCodBarras} con éxito.", id);
            return entidad;
        }

        public async Task<PaginatedResult<MasterDataDTO>> GetTiposCodigoBarraList(int pageNumber, int pageSize, string filtro)
        {
            var query = this.Queryable.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro) && !string.IsNullOrEmpty(filtro))
                query = query.Where(c => c.Descripcion.ToLower().Contains(filtro.ToLower()));

            query = query.OrderBy(p => p.Descripcion);

            var totalRows = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (items == null || !items.Any())
            {
                Log.Error("No se encuentran tipos de codigo de barras");
            }

            Log.Information("OK, se han encontrado {Num} tipos de codigos de barras en el endpoint {Endpoint}", items.Count(), "GetTiposCodigoBarraList");

            return new PaginatedResult<MasterDataDTO>
            {
                Items = _mapper.Map<List<MasterDataDTO>>(items),
                TotalRows = totalRows,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
