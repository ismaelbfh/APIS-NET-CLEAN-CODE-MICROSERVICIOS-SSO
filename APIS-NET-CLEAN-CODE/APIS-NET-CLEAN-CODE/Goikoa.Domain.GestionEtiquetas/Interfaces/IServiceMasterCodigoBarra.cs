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
    public interface IServiceMasterTiposCodigoBarra
    {
        Task<bool> ExisteTipoCodigoBarraEnEtiquetas(int idTipoCodigoBarra);
        Task AddAsync(TiposCodigoBarra entity);
        Task UpdateAsync(TiposCodigoBarra entity);
        Task<PaginatedResult<MasterDataDTO>> GetTiposCodigoBarraList(int pageNumber, int pageSize, string filtro);
        Task<TiposCodigoBarra> DeleteTipoCodigoBarraById(int id);
        Task<List<CodificacionTiposCodigoBarrasDTO>> GetSelectDropdownCodificacionAsync();
    }
}
