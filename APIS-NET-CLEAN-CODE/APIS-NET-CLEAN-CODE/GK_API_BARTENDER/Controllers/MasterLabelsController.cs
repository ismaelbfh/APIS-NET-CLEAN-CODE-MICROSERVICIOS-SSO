using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Goikoa.Domain.GestionEtiquetas.DAL.Models;
using Goikoa.Domain.GestionEtiquetas.DTOs.Requests;
using Goikoa.Domain.GestionEtiquetas.DTOs.Responses;
using Goikoa.Domain.GestionEtiquetas.Entities;
using Goikoa.Domain.GestionEtiquetas.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GK_API_BARTENDER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterLabelsController : ControllerBase
    {
        private readonly IServiceMasterLabels _masterLabelsService;
        private readonly IMapper _mapper;
        public MasterLabelsController(IServiceMasterLabels pMasterLabelsService, IMapper mapper)
        {
            _masterLabelsService = pMasterLabelsService;
            _mapper = mapper;
        }

        [HttpGet("checkLabelEnUso/{idLabel}")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> CheckLabelEnUso(int idLabel)
        {
            var estaEnUso = await _masterLabelsService.ExisteLabelEnEtiquetas(idLabel);
            return Ok(new { exists = estaEnUso });
        }

        [HttpGet("checkLabelDuplicada")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<LabelDuplicadaDTO>> CheckLabelDuplicada(int idTipoLabel, int idIdioma)
        {
            var result = await _masterLabelsService.GetInfoLabelDuplicadaAsync(idTipoLabel, idIdioma);
            return Ok(result);
        }

        [HttpGet("getListLabels")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<LabelDTO>>> GetLabelsList(int pageNumber = 1, int pageSize = 10, string filtro = "")
        {
            var result = await _masterLabelsService.GetLabelsList(pageNumber, pageSize, filtro ?? "");
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpPost("postLabel")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PostLabel(LabelDTORequest pLabelModel)
        {
            var lLabelMap = _mapper.Map<LabelsMaster>(pLabelModel);
            await _masterLabelsService.AddAsync(lLabelMap);
        }

        [HttpPut("putLabel")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PutLabel(LabelDTORequest pLabelModel)
        {
            var lLabelMap = _mapper.Map<LabelsMaster>(pLabelModel);
            await _masterLabelsService.UpdateAsync(lLabelMap);
        }

        [HttpDelete("deleteLabel")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteLabel(int id)
        {
            var label = await _masterLabelsService.DeleteLabelById(id);
            return Ok(label);
        }
    }
}
