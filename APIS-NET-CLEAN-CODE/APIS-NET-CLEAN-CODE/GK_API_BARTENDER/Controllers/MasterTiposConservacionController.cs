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
    public class MasterTiposConservacionController : ControllerBase
    {
        private readonly IServiceMasterTiposConservacion _masterTiposConservacionService;
        private readonly IMapper _mapper;
        public MasterTiposConservacionController(IServiceMasterTiposConservacion pMasterTiposConservacionService, IMapper mapper)
        {
            _masterTiposConservacionService = pMasterTiposConservacionService;
            _mapper = mapper;
        }

        [HttpGet("checkTipoConservacionEnUso/{idTipoConservacion}")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> CheckTipoConservacionEnUso(int idTipoConservacion)
        {
            var estaEnUso = await _masterTiposConservacionService.ConservacionEstaEnUso(idTipoConservacion);
            return Ok(new { exists = estaEnUso });
        }


        [HttpGet("getListTiposConservacion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<MasterDataDTO>>> GetTiposConservacionList(int pageNumber = 1, int pageSize = 10, string filtro = "")
        {
            var result = await _masterTiposConservacionService.GetTiposConservacionList(pageNumber, pageSize, filtro ?? "");
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpPost("postTipoConservacion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PostTipoConservacion(MasterDataDTO pTipoConservacionModel)
        {
            var lTipoConservacionMap = _mapper.Map<TiposConservacion>(pTipoConservacionModel);
            await _masterTiposConservacionService.AddAsync(lTipoConservacionMap);
        }

        [HttpPut("putTipoConservacion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PutTipoConservacion(MasterDataDTO pTipoConservacionModel)
        {
            var lTipoConservacionMap = _mapper.Map<TiposConservacion>(pTipoConservacionModel);
            await _masterTiposConservacionService.UpdateAsync(lTipoConservacionMap);
        }

        [HttpDelete("deleteTipoConservacion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteTipoConservacion(int id)
        {
            var lTipoConserv = await _masterTiposConservacionService.DeleteTipoConservacionById(id);
            return Ok(lTipoConserv);
        }
    }
}
