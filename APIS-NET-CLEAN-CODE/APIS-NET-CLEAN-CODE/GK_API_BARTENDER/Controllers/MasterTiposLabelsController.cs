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
    public class MasterTiposLabelsController : ControllerBase
    {
        private readonly IServiceMasterTiposLabel _masterTiposLabelService;
        private readonly IMapper _mapper;
        public MasterTiposLabelsController(IServiceMasterTiposLabel pMasterTiposLabelService, IMapper mapper)
        {
            _masterTiposLabelService = pMasterTiposLabelService;
            _mapper = mapper;
        }

        [HttpGet("getListTiposLabel")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<MasterDataDTO>>> GetListTiposLabel(int pageNumber = 1, int pageSize = 10, string filtro = "")
        {
            var result = await _masterTiposLabelService.GetTiposLabelsList(pageNumber, pageSize, filtro ?? "");
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpGet("checkTipoLabelEnLabel/{idTipoLabel}")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> CheckTipoLabelEnLabel(int idTipoLabel)
        {
            var existe = await _masterTiposLabelService.ExisteEnLabel(idTipoLabel);
            return Ok(new { exists = existe });
        }

        [HttpPost("postTipoLabel")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PostTipoLabel(MasterDataDTO pTipoLabelModel)
        {
            var lTipoLabelMap = _mapper.Map<TiposLabels>(pTipoLabelModel);
            await _masterTiposLabelService.AddAsync(lTipoLabelMap);
        }

        [HttpPut("putTipoLabel")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PutTipoLabel(MasterDataDTO pTipoLabelModel)
        {
            var lTipoLabelMap = _mapper.Map<TiposLabels>(pTipoLabelModel);
            await _masterTiposLabelService.UpdateAsync(lTipoLabelMap);
        }

        [HttpDelete("deleteTipoLabel")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteTipoLabel(int id)
        {
            var lTipoLabel = await _masterTiposLabelService.DeleteTipoLabelById(id);
            return Ok(lTipoLabel);
        }
    }
}
