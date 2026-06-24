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
    public interface IServiceMasterLabels
    {
        Task<bool> ExisteLabelEnEtiquetas(int idLabel);
        Task AddAsync(LabelsMaster entity);
        Task UpdateAsync(LabelsMaster entity);
        Task<PaginatedResult<LabelDTO>> GetLabelsList(int pageNumber, int pageSize, string filtro);
        Task<LabelsMaster> DeleteLabelById(int id);
        Task<LabelDuplicadaDTO> GetInfoLabelDuplicadaAsync(int idTipoLabel, int idIdioma);
    }
}
