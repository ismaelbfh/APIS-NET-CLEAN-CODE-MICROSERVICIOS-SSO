using System.ComponentModel.DataAnnotations;
using Goikoa.Domain.Navision.Producccion.Entities;
using Goikoa.Domain.Navision.Producccion.DTOs.Requests;
using Goikoa.Domain.Navision.Producccion.DTOs.Responses;
using Goikoa.Domain.Navision.Producccion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GK_API_NAV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenesNavisionController : ControllerBase
    {
        private readonly INavisionService _navService;

        public OrdenesNavisionController(INavisionService pNavService)
        {
            _navService = pNavService;
        }

        [HttpGet("getOrdenesProduccion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<PaginatedResult<GKOrdenProdDTO>?> GetOrdenesProduccion([FromQuery] GKOrdenProdRequest pRequest, [FromQuery] string? filtro = null)
        {
            PaginatedResult<GKOrdenProdDTO>? lLstOrdenes = await _navService.GetOrdenesByLineaDestinoFecha(pRequest.pLine, pRequest.pTipoDestino, pRequest.pStartingDate, pRequest.pPageNumber, pRequest.pPageSize, filtro);
            return lLstOrdenes;
        }

        [HttpGet("getPrevisionesPorFecha")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<PaginatedResult<GKPrevisionDTO>?> GetPrevisionesPorFecha([FromQuery] DateTime fecha, [FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? filtro = null)
        {
            PaginatedResult<GKPrevisionDTO>? resultado = await _navService.GetPrevisionesPorFechaAsync(fecha, pageNumber, pageSize, filtro);
            return resultado;
        }

        [HttpGet("getDatosOrdenProduccion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<GKInfoOrdenCamaraDTO?> GetDatosOrdenProduccion([FromQuery, Required] string pOPFabricacion)
        {
            GKInfoOrdenCamaraDTO? lOrden = await _navService.GetOrdenFabricacion(pOPFabricacion);
            return lOrden;
        }

        [HttpGet("getCodigoBarras")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<string> GetCodificacionCodigoBarras([FromQuery] GKProductoRequest pRequest)
        {
            GKProductoDTO? lProducto = await _navService.GetCodigoBarrasByProductAndType(pRequest.pNumProduct, pRequest.pTipoCodigoBarras);
            return lProducto.CodificacionCodigoBarras;
        }

        [HttpGet("getCodigoQr")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<string> GetCodigoQr([FromQuery] string pNumProduct, [FromQuery] string pQrType)
        {
            return await _navService.GetCodigoQrByProduct(pNumProduct, pQrType);
        }
    }
}
