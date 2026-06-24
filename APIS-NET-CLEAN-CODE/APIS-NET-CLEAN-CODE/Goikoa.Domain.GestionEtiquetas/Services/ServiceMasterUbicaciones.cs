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
    public class ServiceMasterUbicaciones : Repository<Ubicaciones>, IServiceMasterUbicaciones
    {
        private readonly IMapper _mapper;
        private readonly BDGestionEtiquetasContext _dbContext;
        public ServiceMasterUbicaciones(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> EstaUbicacionEnUsoAsync(int idUbicacion)
        {
            return await _dbContext.Etiqueta.AnyAsync(e => 
                e.FK_IdUbicacion == idUbicacion &&
                (e.IsDeleted == false || e.IsDeleted == null)
            );
        }

        public async Task<PaginatedResult<MasterDataDTO>> GetUbicacionesList(int pageNumber, int pageSize, string filtro)
        {
            var query = this.Queryable.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro) && !string.IsNullOrEmpty(filtro))
                query = query.Where(c => c.Descripcion.ToLower().Contains(filtro.ToLower()));

            query = query.OrderBy(p => p.Descripcion);

            var totalRows = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (items == null || !items.Any())
            {
                Log.Error("No se encuentran ubicaciones");
            }

            Log.Information("OK, se han encontrado {NumUbicaciones} ubicaciones en el endpoint {Endpoint}", items.Count(), "GetUbicacionesList");

            return new PaginatedResult<MasterDataDTO>
            {
                Items = _mapper.Map<List<MasterDataDTO>>(items),
                TotalRows = totalRows,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Ubicaciones> DeleteUbicacionById(int id)
        {
            // Obtiene la entidad desde el DbContext (está siendo rastreada)
            var entidad = await _dbContext.Ubicaciones.FirstOrDefaultAsync(x => x.PK_IdUbicacion == id);

            if (entidad == null)
            {
                Log.Error("Error al borrar una ubicacion porque el id {IdUbicacion} no existe.", id);
                throw new KeyNotFoundException($"No se encontró el cliente con id {id}.");
            }

            // Remueve la entidad del contexto y guarda los cambios
            _dbContext.Ubicaciones.Remove(entidad);
            try
            {
                // Paso 4: Guardar cambios y capturar errores
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para borrar una ubicacion: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para borrar una ubicacion: {inner}", ex);
            }
            Log.Information("Se ha eliminado una ubicacion con id {IdUbicacion} con éxito.", id);
            return entidad;
        }
    }
}
