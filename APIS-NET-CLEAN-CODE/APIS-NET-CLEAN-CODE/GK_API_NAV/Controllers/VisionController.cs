using Goikoa.Domain.ApiAdmin.DTOs.Requests;
using Goikoa.Domain.ApiAdmin.DTOs.Responses;
using Goikoa.Domain.ApiAdmin.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GK_API_NAV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisionController : ControllerBase
    {
        private readonly IVisionService _visionService;

        public VisionController(IVisionService pVisionService)
        {
            _visionService = pVisionService;
        }

        [HttpPost("iniciarOrden")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<VisionOrdenResumenDTO> IniciarOrden([FromBody] VisionIniciarOrdenRequest pRequest)
        {
            return await _visionService.IniciarOrdenAsync(pRequest);
        }

        [HttpPost("registrarLectura")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<VisionLecturaDTO> RegistrarLectura([FromBody] VisionRegistrarLecturaRequest pRequest)
        {
            return await _visionService.RegistrarLecturaAsync(pRequest);
        }

        [HttpGet("getResumenByOrden")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<VisionOrdenResumenDTO> GetResumenByOrden([FromQuery] string ordenFabricacion)
        {
            return await _visionService.GetResumenByOrdenAsync(ordenFabricacion);
        }

        [HttpPost("finalizarOrden")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<VisionOrdenResumenDTO> FinalizarOrden([FromBody] VisionFinalizarOrdenRequest pRequest)
        {
            return await _visionService.FinalizarOrdenAsync(pRequest);
        }

        [HttpGet("getLecturasByResumenId")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<List<VisionLecturaDTO>> GetLecturasByResumenId([FromQuery] Guid visionOrdenResumenId)
        {
            return await _visionService.GetLecturasByResumenIdAsync(visionOrdenResumenId);
        }

        [HttpGet("getResumenesByOrden")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<List<VisionOrdenResumenDTO>> GetResumenesByOrden([FromQuery] string ordenFabricacion)
        {
            return await _visionService.GetResumenesByOrdenAsync(ordenFabricacion);
        }

        [HttpPost("actualizarOrden")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<VisionOrdenResumenDTO> ActualizarOrden([FromBody] VisionActualizarOrdenRequest pRequest)
        {
            return await _visionService.ActualizarOrdenAsync(pRequest);
        }
    }
}