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
    public interface IServiceMasterCampo
    {
        Task<bool> ExisteEnPlantilla(int idCampo);
        Task AddAsync(Campo entity);
        Task UpdateAsync(Campo entity);
        Task<PaginatedResult<CampoDTO>> GetCamposList(int pageNumber, int pageSize, string filtro);
        Task<Campo> DeleteCampoById(int id);
    }
}
