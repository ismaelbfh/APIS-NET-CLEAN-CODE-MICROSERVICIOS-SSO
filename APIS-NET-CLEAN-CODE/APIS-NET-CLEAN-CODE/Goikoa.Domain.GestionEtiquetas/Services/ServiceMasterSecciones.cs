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
    public class ServiceMasterSecciones : Repository<Secciones>, IServiceMasterSecciones
    {
        private readonly IMapper _mapper;
        private readonly BDGestionEtiquetasContext _dbContext;
        public ServiceMasterSecciones(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> CheckSeccionEnUso(int idSeccion)
        {
            return await _dbContext.Etiqueta.AnyAsync(e => 
                e.FK_IdSeccion == idSeccion &&
                (e.IsDeleted == false || e.IsDeleted == null)
            );
        }

        public async Task<PaginatedResult<MasterDataDTO>> GetSeccionesList(int pageNumber, int pageSize, string filtro)
        {
            var query = this.Queryable.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro) && !string.IsNullOrEmpty(filtro))
                query = query.Where(c => c.Descripcion.ToLower().Contains(filtro.ToLower()));

            query = query.OrderBy(p => p.Descripcion);

            var totalRows = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            Log.Information("OK, se han encontrado {NumTiposSeccion} tipos de seccion en el endpoint {Endpoint}", items.Count(), "GetTiposCamposList");

            return new PaginatedResult<MasterDataDTO>
            {
                Items = _mapper.Map<List<MasterDataDTO>>(items ?? new List<Secciones>()),
                TotalRows = totalRows,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Secciones> DeleteSeccionById(int id)
        {
            // Obtiene la entidad desde el DbContext (está siendo rastreada)
            var entidad = await _dbContext.Secciones.FirstOrDefaultAsync(x => x.PK_IdSeccion == id);

            if (entidad == null)
            {
                Log.Error("Error al borrar una seccion porque el id {IdSeccion} no existe.", id);
                throw new KeyNotFoundException($"No se encontró la Sección con id {id}.");
            }
            
            // Remueve la entidad del contexto y guarda los cambios
            _dbContext.Secciones.Remove(entidad);
            try
            {
                // Paso 4: Guardar cambios y capturar errores
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para borrar una seccion: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para borrar una seccion: {inner}", ex);
            }
            Log.Information("Se ha eliminado la seccion con id {IdSeccion} con éxito.", id);
            return entidad;
        }
    }
}
