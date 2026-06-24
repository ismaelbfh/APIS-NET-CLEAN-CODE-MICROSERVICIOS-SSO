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
    public class ServiceCamposNavision : Repository<CamposNavision>, IServiceCamposNavision
    {
        private readonly IMapper _mapper;
        private readonly BDGestionEtiquetasContext _dbContext;

        public ServiceCamposNavision(BDGestionEtiquetasContext dbContext, IMapper mapper) : base(dbContext)
        {
            _mapper = mapper;
            _dbContext = dbContext;
        }

        public async Task<PaginatedResult<CamposNavisionDTO>> GetCamposNavisionList(int pageNumber, int pageSize, string filtro)
        {
            var query = this.Queryable.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                string texto = filtro.Trim().ToLower();
                query = query.Where(cn => cn.Descripcion.ToLower().Contains(texto));
            }

            query = query.OrderBy(cn => cn.PK_IdCampoNavision);

            var totalRows = await query.CountAsync();
            var lLstCamposNavision = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (lLstCamposNavision == null || !lLstCamposNavision.Any())
            {
                Log.Warning("No se encuentran campos de Navision para el filtro: {Filtro}", filtro);
            }

            Log.Information("OK, se han encontrado {NumCamposNavision} campos de Navision.", lLstCamposNavision.Count());

            var result = new PaginatedResult<CamposNavisionDTO>
            {
                Items = _mapper.Map<List<CamposNavisionDTO>>(lLstCamposNavision),
                TotalRows = totalRows,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return result;
        }

        public async Task<CamposNavisionDTO> GetCampoNavisionById(int id)
        {
            var campoNavision = this.Queryable.AsNoTracking().FirstOrDefault(x => x.PK_IdCampoNavision == id);
            if (campoNavision == null)
            {
                
                Log.Error("Campo de Navision con ID {Id} no encontrado.", id);
                throw new KeyNotFoundException($"Campo de Navision con ID {id} no encontrado.");
            }
            return _mapper.Map<CamposNavisionDTO>(campoNavision);
        }
    }
}
