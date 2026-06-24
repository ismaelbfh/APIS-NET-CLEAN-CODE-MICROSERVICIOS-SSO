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
    public interface IServiceMasterSecciones
    {
        Task<bool> CheckSeccionEnUso(int idSeccion);
        Task AddAsync(Secciones entity);
        Task UpdateAsync(Secciones entity);
        Task<PaginatedResult<MasterDataDTO>> GetSeccionesList(int pageNumber, int pageSize, string filtro);
        Task<Secciones> DeleteSeccionById(int id);
    }
}
