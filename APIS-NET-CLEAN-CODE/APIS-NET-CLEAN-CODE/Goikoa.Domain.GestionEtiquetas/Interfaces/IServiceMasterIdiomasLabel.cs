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
    public interface IServiceMasterIdiomasLabel
    {
        Task<bool> ExisteIdiomaEnLabel(int idIdiomaLabel);
        Task AddAsync(IdiomasLabels entity);
        Task UpdateAsync(IdiomasLabels entity);
        Task<PaginatedResult<MasterDataDTO>> GetIdiomasLabelsList(int pageNumber, int pageSize, string filtro);
        Task<IdiomasLabels> DeleteIdiomaLabelById(int id);
    }
}
