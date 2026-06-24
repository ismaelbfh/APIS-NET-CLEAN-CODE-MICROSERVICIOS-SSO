using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Goikoa.Domain.GestionEtiquetas.DAL.Models;
using Goikoa.Domain.GestionEtiquetas.DTOs.Requests;
using Goikoa.Domain.GestionEtiquetas.DTOs.Responses;
using Goikoa.Domain.GestionEtiquetas.Entities;
using Goikoa.Domain.GestionEtiquetas.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GK_API_BARTENDER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MasterCamposController : ControllerBase
    {
        private readonly IServiceMasterCampo _masterCamposService;
        private readonly IMapper _mapper;
        public MasterCamposController(IServiceMasterCampo pMasterCamposService, IMapper mapper)
        {
            _masterCamposService = pMasterCamposService;
            _mapper = mapper;
        }

        [HttpGet("getListCampos")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<CampoDTO>>> GetListCampos(int pageNumber = 1, int pageSize = 10, string filtro = "")
        {
            var lCamposResponse = await _masterCamposService.GetCamposList(pageNumber, pageSize, filtro ?? "");
            if (lCamposResponse == null || lCamposResponse.Items == null || lCamposResponse.Items.Count == 0)
                return NoContent();
            return Ok(lCamposResponse);
        }

        [HttpPost("postCampo")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PostCampo(CampoDTORequest pCampoModel)
        {
            var lCampoMap = _mapper.Map<Campo>(pCampoModel);
            await _masterCamposService.AddAsync(lCampoMap);
        }

        [HttpGet("checkCampoEnPlantillas/{idCampo}")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> CheckCampoEnPlantillas(int idCampo)
        {
            var existe = await _masterCamposService.ExisteEnPlantilla(idCampo);
            return Ok(new { exists = existe });
        }

        [HttpPut("putCampo")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PutCampo(CampoDTORequest pCampoModel)
        {
            var lCampoMap = _mapper.Map<Campo>(pCampoModel);
            await _masterCamposService.UpdateAsync(lCampoMap);
        }

        [HttpDelete("deleteCampo")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteCampo(int id)
        {
            var lCampo = await _masterCamposService.DeleteCampoById(id);
            return Ok(lCampo);
        }
    }
}
