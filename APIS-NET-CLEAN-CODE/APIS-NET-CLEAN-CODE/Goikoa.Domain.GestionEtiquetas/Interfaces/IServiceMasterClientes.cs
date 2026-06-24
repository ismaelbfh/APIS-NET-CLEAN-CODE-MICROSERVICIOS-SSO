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
    public interface IServiceMasterClientes
    {
        Task<bool> EstaClienteEnUsoAsync(int idCliente);
        Task AddAsync(Cliente entity);
        Task UpdateAsync(Cliente entity);
        Task<PaginatedResult<MasterDataDTO>> GetClientesList(int pageNumber, int pageSize, string filtro);
        Task<Cliente> DeleteClienteById(int id);
    }
}
