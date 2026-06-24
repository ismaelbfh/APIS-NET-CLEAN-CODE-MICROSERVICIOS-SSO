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
    public class MasterIdiomasLabelsController : ControllerBase
    {
        private readonly IServiceMasterIdiomasLabel _masterIdiomasLabelService;
        private readonly IMapper _mapper;
        public MasterIdiomasLabelsController(IServiceMasterIdiomasLabel pMasterIdiomasLabelService, IMapper mapper)
        {
            _masterIdiomasLabelService = pMasterIdiomasLabelService;
            _mapper = mapper;
        }

        [HttpGet("getListIdiomasLabel")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<MasterDataDTO>>> GetListIdiomasLabel(int pageNumber = 1, int pageSize = 10, string filtro = "")
        {
            var result = await _masterIdiomasLabelService.GetIdiomasLabelsList(pageNumber, pageSize, filtro ?? "");
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpGet("checkIdiomaLabelEnLabel/{idIdiomaLabel}")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> CheckIdiomaLabelEnLabel(int idIdiomaLabel)
        {
            var existe = await _masterIdiomasLabelService.ExisteIdiomaEnLabel(idIdiomaLabel);
            return Ok(new { exists = existe });
        }

        [HttpPost("postIdiomaLabel")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PostIdiomaLabel(MasterDataDTO pIdiomaLabelModel)
        {
            var lIdiomaLabelMap = _mapper.Map<IdiomasLabels>(pIdiomaLabelModel);
            await _masterIdiomasLabelService.AddAsync(lIdiomaLabelMap);
        }

        [HttpPut("putIdiomaLabel")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PutIdiomaLabel(MasterDataDTO pIdiomaLabelModel)
        {
            var lIdiomaLabelMap = _mapper.Map<IdiomasLabels>(pIdiomaLabelModel);
            await _masterIdiomasLabelService.UpdateAsync(lIdiomaLabelMap);
        }

        [HttpDelete("deleteIdiomaLabel")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteIdiomaLabel(int id)
        {
            var lIdiomaLabel = await _masterIdiomasLabelService.DeleteIdiomaLabelById(id);
            return Ok(lIdiomaLabel);
        }
    }
}
