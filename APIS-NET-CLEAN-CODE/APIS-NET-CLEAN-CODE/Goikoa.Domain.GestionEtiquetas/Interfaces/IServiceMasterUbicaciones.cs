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
    public interface IServiceMasterUbicaciones
    {
        Task<bool> EstaUbicacionEnUsoAsync(int idUbicacion);
        Task AddAsync(Ubicaciones entity);
        Task UpdateAsync(Ubicaciones entity);
        Task<PaginatedResult<MasterDataDTO>> GetUbicacionesList(int pageNumber, int pageSize, string filtro);
        Task<Ubicaciones> DeleteUbicacionById(int id);
    }
}
