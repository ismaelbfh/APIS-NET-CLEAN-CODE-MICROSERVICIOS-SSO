using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Goikoa.Domain.GestionEtiquetas.DAL.Models;
using Goikoa.Domain.GestionEtiquetas.DTOs.Responses;
using Goikoa.Domain.GestionEtiquetas.Entities;
using Goikoa.Domain.GestionEtiquetas.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GK_API_BARTENDER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterUbicacionesController : ControllerBase
    {
        private readonly IServiceMasterUbicaciones _masterUbicacionesService;
        private readonly IMapper _mapper;
        public MasterUbicacionesController(IServiceMasterUbicaciones pMasterClientesService, IMapper mapper)
        {
            _masterUbicacionesService = pMasterClientesService;
            _mapper = mapper;
        }

        [HttpGet("checkUbicacionEnUso/{idUbicacion}")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> CheckClienteEnUso(int idCliente)
        {
            var enUso = await _masterUbicacionesService.EstaUbicacionEnUsoAsync(idCliente);
            return Ok(new { exists = enUso });
        }

        [HttpGet("getListUbicaciones")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<MasterDataDTO>>> GetClientesList(int pageNumber = 1, int pageSize = 10, string filtro = "")
        {
            var result = await _masterUbicacionesService.GetUbicacionesList(pageNumber, pageSize, filtro ?? "");
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpPost("postUbicacion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PostUbicacion(MasterDataDTO pUbicacionModel)
        {
            var lUbicacionMap = _mapper.Map<Ubicaciones>(pUbicacionModel);
            await _masterUbicacionesService.AddAsync(lUbicacionMap);
        }

        [HttpPut("putUbicacion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PutUbicacion(MasterDataDTO pUbicacionModel)
        {
            var lUbicacionMap = _mapper.Map<Ubicaciones>(pUbicacionModel);
            await _masterUbicacionesService.UpdateAsync(lUbicacionMap);
        }

        [HttpDelete("deleteUbicacion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteUbicacion(int id)
        {
            var lUbicacion = await _masterUbicacionesService.DeleteUbicacionById(id);
            return Ok(lUbicacion);
        }
    }
}
