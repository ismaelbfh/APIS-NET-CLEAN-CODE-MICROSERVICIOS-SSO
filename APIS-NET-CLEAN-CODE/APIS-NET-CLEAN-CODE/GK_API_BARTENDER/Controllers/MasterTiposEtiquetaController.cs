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
    public class MasterTiposEtiquetaController : ControllerBase
    {
        private readonly IServiceMasterTiposEtiqueta _masterTiposEtiquetaService;
        private readonly IMapper _mapper;
        public MasterTiposEtiquetaController(IServiceMasterTiposEtiqueta pMasterTiposEtiquetaService, IMapper mapper)
        {
            _masterTiposEtiquetaService = pMasterTiposEtiquetaService;
            _mapper = mapper;
        }

        [HttpGet("checkTipoEtiquetaEnUso/{idTipoEtiqueta}")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> CheckTipoEtiquetaEnUso(int idTipoEtiqueta)
        {
            var estaEnUso = await _masterTiposEtiquetaService.ExisteTipoEtiquetaEnEtiquetas(idTipoEtiqueta);
            return Ok(new { exists = estaEnUso });
        }

        [HttpGet("getListTiposEtiquetas")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<MasterDataDTO>>> GetTiposEtiquetasList(int pageNumber = 1, int pageSize = 10, string filtro = "")
        {
            var result = await _masterTiposEtiquetaService.GetTiposEtiquetasList(pageNumber, pageSize, filtro ?? "");
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpPost("postTipoEtiqueta")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PostTipoEtiqueta(MasterDataDTO pTipoEtiquetaModel)
        {
            var lTipoEtiquetaMap = _mapper.Map<TiposEtiqueta>(pTipoEtiquetaModel);
            await _masterTiposEtiquetaService.AddAsync(lTipoEtiquetaMap);
        }

        [HttpPut("putTipoEtiqueta")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PutTipoEtiqueta(MasterDataDTO pTipoEtiquetaModel)
        {
            var lTipoEtiquetaMap = _mapper.Map<TiposEtiqueta>(pTipoEtiquetaModel);
            await _masterTiposEtiquetaService.UpdateAsync(lTipoEtiquetaMap);
        }

        [HttpDelete("deleteTipoEtiqueta")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteTipoEtiqueta(int id)
        {
            var tipoEtiqueta = await _masterTiposEtiquetaService.DeleteTipoEtiquetaById(id);
            return Ok(tipoEtiqueta);
        }
    }
}
