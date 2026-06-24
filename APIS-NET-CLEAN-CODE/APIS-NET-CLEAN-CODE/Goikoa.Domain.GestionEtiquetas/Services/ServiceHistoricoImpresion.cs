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
    public class ServiceHistoricoImpresion : Repository<HistoricoImpresion>, IServiceHistoricoImpresion
    {
        private readonly BDGestionEtiquetasContext _dbContext;
        private readonly IMapper _mapper;
        public ServiceHistoricoImpresion(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<HistoricoImpresionDTO>> GetHistoricosList(int pageNumber, int pageSize, string filtro)
        {
            var query = _dbContext.HistoricoImpresion.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro) && !string.IsNullOrEmpty(filtro))
            {
                string texto = filtro.ToLower().Trim();

                // Intenta parsear como fecha (acepta varios formatos)
                DateTime? fechaBuscada = null;
                if (DateTime.TryParseExact(filtro, new[] { "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy" },
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var dt))
                {
                    fechaBuscada = dt.Date;
                }

                query = query.Where(h =>
                    // CódigoProducto y CódigoOrden: filtro exacto (case insensitive)
                    (h.CodigoProducto != null && h.CodigoProducto.ToLower() == texto) ||
                    (h.CodigoOrden != null && h.CodigoOrden.ToLower() == texto) ||

                    // NombreProducto y NombreEtiqueta: filtro parcial
                    (h.NombreEtiqueta != null && h.NombreEtiqueta.ToLower().Contains(texto)) ||

                    // Resto: exacto (sin contains)
                    (h.NombrePlantilla != null && h.NombrePlantilla.ToLower() == texto) ||
                    (h.TipoEtiqueta != null && h.TipoEtiqueta.ToLower() == texto) ||
                    (h.TipoConservacion != null && h.TipoConservacion.ToLower() == texto) ||
                    (h.Seccion != null && h.Seccion.ToLower() == texto) ||
                    (h.Cliente != null && h.Cliente.ToLower() == texto) ||
                    (h.Envasado != null && h.Envasado.ToLower() == texto) ||
                    (h.TipoCodigoBarras != null && h.TipoCodigoBarras.ToLower() == texto) ||
                    (h.OvaloIdioma != null && h.OvaloIdioma.ToLower() == texto) ||
                    (h.OvaloPlanta != null && h.OvaloPlanta.ToLower() == texto) ||
                    (h.OvaloCE != null && h.OvaloCE.ToLower() == texto) ||
                    (h.UsuarioPlanta != null && h.UsuarioPlanta.ToLower() == texto) ||
                    (h.UsuarioEtiqueta != null && h.UsuarioEtiqueta.ToLower() == texto) ||
                    (h.IpMaquina != null && h.IpMaquina.ToLower() == texto) ||

                    // Fecha
                    (fechaBuscada.HasValue && h.FechaImpresion.Date == fechaBuscada.Value)
                );
            }

            var total = await query.CountAsync();

            var entidades = await query
                .OrderByDescending(h => h.FechaImpresion)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<HistoricoImpresionDTO>>(entidades);

            return new PaginatedResult<HistoricoImpresionDTO>
            {
                Items = dtos,
                TotalRows = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }


        public async Task AddHistoricoImpresionAsync(HistoricoImpresionDTO dto)
        {

            var entity = _mapper.Map<HistoricoImpresion>(dto);
            await _dbContext.HistoricoImpresion.AddAsync(entity);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(
                    $"Error al guardar el histórico de impresión. Detalle: {ex.InnerException?.Message ?? ex.Message}",
                    ex
                );
            }

        }
    }
}
