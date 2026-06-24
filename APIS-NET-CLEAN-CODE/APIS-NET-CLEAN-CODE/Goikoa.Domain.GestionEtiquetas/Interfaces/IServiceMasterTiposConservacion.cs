using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goikoa.Domain.GestionEtiquetas.DAL.Models;
using Goikoa.Domain.GestionEtiquetas.DTOs.Responses;
using Goikoa.Domain.GestionEtiquetas.Entities;

namespace Goikoa.Domain.GestionEtiquetas.Interfaces
{
    public interface IServiceMasterTiposConservacion
    {
        Task<bool> ConservacionEstaEnUso(int idTipoConservacion);
        Task AddAsync(TiposConservacion entity);
        Task UpdateAsync(TiposConservacion entity);
        Task<PaginatedResult<MasterDataDTO>> GetTiposConservacionList(int pageNumber, int pageSize, string filtro);
        Task<TiposConservacion> DeleteTipoConservacionById(int id);
    }
}
