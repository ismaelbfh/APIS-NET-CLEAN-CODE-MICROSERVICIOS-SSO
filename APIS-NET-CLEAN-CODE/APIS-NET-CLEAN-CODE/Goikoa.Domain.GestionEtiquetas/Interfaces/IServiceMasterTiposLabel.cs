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
    public interface IServiceMasterTiposLabel
    {
        Task<bool> ExisteEnLabel(int idTipoLabel);
        Task AddAsync(TiposLabels entity);
        Task UpdateAsync(TiposLabels entity);
        Task<PaginatedResult<MasterDataDTO>> GetTiposLabelsList(int pageNumber, int pageSize, string filtro);
        Task<TiposLabels> DeleteTipoLabelById(int id);
    }
}
