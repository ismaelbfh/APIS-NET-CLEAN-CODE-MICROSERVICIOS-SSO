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
    public interface IServiceEtiqueta
    {
        Task<PaginatedResult<EtiquetaDTO>> GetEtiquetasList(int pageNumber, int pageSize, string filtro, int? plantillaId);
        Task<Etiqueta> GetEtiquetaPorCodigoProductoAsync(string pCodigoProducto);
        Task AddEtiquetaAsync(EtiquetaDTO etiquetaDto);
        Task UpdateEtiquetaAsync(EtiquetaDTO etiquetaDto);
        Task<Etiqueta> DeleteEtiquetaAsync(int id);
    }
}
