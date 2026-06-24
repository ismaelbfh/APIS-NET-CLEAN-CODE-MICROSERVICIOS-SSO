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
    public interface IServiceMasterTiposCampo
    {
        Task<bool> ExisteEnCampo(int idTipoCampo);
        Task AddAsync(TiposCampo entity);
        Task UpdateAsync(TiposCampo entity);
        Task<PaginatedResult<MasterDataDTO>> GetTiposCamposList(int pageNumber, int pageSize, string filtro);
        Task<TiposCampo> DeleteTipoCampoById(int id);
    }
}
