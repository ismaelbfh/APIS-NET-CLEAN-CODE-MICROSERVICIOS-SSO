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
    public class ServiceMasterClientes : Repository<Cliente>, IServiceMasterClientes
    {
        private readonly IMapper _mapper;
        private readonly BDGestionEtiquetasContext _dbContext;
        public ServiceMasterClientes(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<bool> EstaClienteEnUsoAsync(int idCliente)
        {
            return await _dbContext.Etiqueta.AnyAsync(e => 
                e.FK_IdCliente == idCliente &&
                (e.IsDeleted == false || e.IsDeleted == null)
            );
        }

        public async Task<PaginatedResult<MasterDataDTO>> GetClientesList(int pageNumber, int pageSize, string filtro)
        {
            var query = this.Queryable.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro) && !string.IsNullOrEmpty(filtro))
                query = query.Where(c => c.Descripcion.ToLower().Contains(filtro.ToLower()));

            query = query.OrderBy(p => p.Descripcion);

            var totalRows = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (items == null || !items.Any())
            {
                Log.Error("No se encuentran tipos de clientes");
            }

            Log.Information("OK, se han encontrado {NumTiposClientes} tipos de clientes en el endpoint {Endpoint}", items.Count(), "GetClientesList");

            return new PaginatedResult<MasterDataDTO>
            {
                Items = _mapper.Map<List<MasterDataDTO>>(items),
                TotalRows = totalRows,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Cliente> DeleteClienteById(int id)
        {
            // Obtiene la entidad desde el DbContext (está siendo rastreada)
            var entidad = await _dbContext.Cliente.FirstOrDefaultAsync(x => x.PK_IdCliente == id);

            if (entidad == null)
            {
                Log.Error("Error al borrar un cliente porque el id {IdCliente} no existe.", id);
                throw new KeyNotFoundException($"No se encontró el cliente con id {id}.");
            }

            // Remueve la entidad del contexto y guarda los cambios
            _dbContext.Cliente.Remove(entidad);
            try
            {
                // Paso 4: Guardar cambios y capturar errores
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                var inner = ex.InnerException?.Message;
                Log.Error("Error al guardar los cambios en la base de datos para borrar un cliente: {inner} , excepcion completa: {err}", inner, ex);
                throw new Exception($"Error al guardar los cambios en la base de datos para borrar un cliente: {inner}", ex);
            }
            Log.Information("Se ha eliminado un cliente con id {IdCliente} con éxito.", id);
            return entidad;
        }
    }
}
