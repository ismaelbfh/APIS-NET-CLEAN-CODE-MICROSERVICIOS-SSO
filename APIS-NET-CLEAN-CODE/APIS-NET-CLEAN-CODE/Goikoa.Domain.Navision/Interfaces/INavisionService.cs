using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goikoa.Domain.Navision.Producccion.DTOs.Responses;
using Goikoa.Domain.Navision.Producccion.Entities;

namespace Goikoa.Domain.Navision.Producccion.Interfaces
{
    public interface INavisionService
    {
        Task<GKInfoOrdenCamaraDTO> GetOrdenFabricacion(string pOPFabricacion);
        Task<GKProductoDTO?> GetCodigoBarrasByProductAndType(string pNumProduct, string pTipoCodigoBarras);
        Task<PaginatedResult<GKOrdenProdDTO>> GetOrdenesByLineaDestinoFecha(string pLinea, int pTipoDestino, DateTime pFecha, int pPageNumber, int pPageSize, string? filtro);
        Task<PaginatedResult<GKPrevisionDTO>> GetPrevisionesPorFechaAsync(DateTime pFecha, int pPageNumber, int pPageSize, string? filtro);
        Task<string> GetCodigoQrByProduct(string pNumProduct, string pQrType);
    }
}
