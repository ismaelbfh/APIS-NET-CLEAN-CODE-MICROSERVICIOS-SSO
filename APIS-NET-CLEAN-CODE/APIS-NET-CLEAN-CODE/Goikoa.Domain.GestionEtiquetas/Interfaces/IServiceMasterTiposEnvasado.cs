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
    public interface IServiceMasterTiposEnvasado
    {
        Task<bool> CheckTipoEnvasadoEnUso(int idTipoEnvasado);
        Task AddAsync(TiposEnvasado entity);
        Task UpdateAsync(TiposEnvasado entity);
        Task<PaginatedResult<MasterDataDTO>> GetTiposEnvasadoList(int pageNumber, int pageSize, string filtro);
        Task<TiposEnvasado> DeleteTipoEnvasadoById(int id);
    }
}
