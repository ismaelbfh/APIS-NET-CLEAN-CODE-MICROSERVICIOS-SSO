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
    public class MasterTiposCamposController : ControllerBase
    {
        private readonly IServiceMasterTiposCampo _masterTiposCamposService;
        private readonly IMapper _mapper;
        public MasterTiposCamposController(IServiceMasterTiposCampo pMasterTiposCampoService, IMapper mapper)
        {
            _masterTiposCamposService = pMasterTiposCampoService;
            _mapper = mapper;
        }

        [HttpGet("getListTiposCampo")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<MasterDataDTO>>> GetListTiposCampo(int pageNumber = 1, int pageSize = 10, string filtro = "")
        {
            var result = await _masterTiposCamposService.GetTiposCamposList(pageNumber, pageSize, filtro ?? "");
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpGet("checkTipoCampoEnCampos/{idTipoCampo}")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> CheckTipoCampoEnCampos(int idTipoCampo)
        {
            var existe = await _masterTiposCamposService.ExisteEnCampo(idTipoCampo);
            return Ok(new { exists = existe });
        }

        [HttpPost("postTipoCampo")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PostTipoCampo(MasterDataDTO pTipoCampoModel)
        {
            var lCampoMap = _mapper.Map<TiposCampo>(pTipoCampoModel);
            await _masterTiposCamposService.AddAsync(lCampoMap);
        }

        [HttpPut("putTipoCampo")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PutTipoCampo(MasterDataDTO pTipoCampoModel)
        {
            var lCampoMap = _mapper.Map<TiposCampo>(pTipoCampoModel);
            await _masterTiposCamposService.UpdateAsync(lCampoMap);
        }

        [HttpDelete("deleteTipoCampo")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteTipoCampo(int id)
        {
            var lTipoCampo = await _masterTiposCamposService.DeleteTipoCampoById(id);
            return Ok(lTipoCampo);
        }
    }
}
