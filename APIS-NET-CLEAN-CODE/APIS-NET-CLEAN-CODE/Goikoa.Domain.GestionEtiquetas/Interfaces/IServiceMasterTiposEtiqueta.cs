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
    public interface IServiceMasterTiposEtiqueta
    {
        Task<bool> ExisteTipoEtiquetaEnEtiquetas(int idTipoEtiqueta);
        Task AddAsync(TiposEtiqueta entity);
        Task UpdateAsync(TiposEtiqueta entity);
        Task<PaginatedResult<MasterDataDTO>> GetTiposEtiquetasList(int pageNumber, int pageSize, string filtro);
        Task<TiposEtiqueta> DeleteAsync(TiposEtiqueta entity);
        Task<TiposEtiqueta> DeleteTipoEtiquetaById(int id);
    }
}
