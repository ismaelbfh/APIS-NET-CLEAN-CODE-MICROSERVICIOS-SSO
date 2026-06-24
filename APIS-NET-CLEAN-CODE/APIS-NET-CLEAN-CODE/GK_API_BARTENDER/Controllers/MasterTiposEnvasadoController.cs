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
    public class MasterTiposEnvasadoController : ControllerBase
    {
        private readonly IServiceMasterTiposEnvasado _masterTiposEnvasadoService;
        private readonly IMapper _mapper;
        public MasterTiposEnvasadoController(IServiceMasterTiposEnvasado pMasterTiposEnvasadoService, IMapper mapper)
        {
            _masterTiposEnvasadoService = pMasterTiposEnvasadoService;
            _mapper = mapper;
        }

        [HttpGet("checkTipoEnvasadoEnUso/{idTipoEnvasado}")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> CheckTipoEnvasadoEnUso(int idTipoEnvasado)
        {
            var enUso = await _masterTiposEnvasadoService.CheckTipoEnvasadoEnUso(idTipoEnvasado);
            return Ok(new { exists = enUso });
        }

        [HttpGet("getListTiposEnvasado")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<MasterDataDTO>>> GetTiposEnvasadoList(int pageNumber = 1, int pageSize = 10, string filtro = "")
        {
            var result = await _masterTiposEnvasadoService.GetTiposEnvasadoList(pageNumber, pageSize, filtro ?? "");
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpPost("postTipoEnvasado")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PostTipoEnvasado(MasterDataDTO pTipoEnvasadoModel)
        {
            var lTipoEnvasadoMap = _mapper.Map<TiposEnvasado>(pTipoEnvasadoModel);
            await _masterTiposEnvasadoService.AddAsync(lTipoEnvasadoMap);
        }

        [HttpPut("putTipoEnvasado")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PutTipoEnvasado(MasterDataDTO pTipoEnvasadoModel)
        {
            var lTipoEnvasadoMap = _mapper.Map<TiposEnvasado>(pTipoEnvasadoModel);
            await _masterTiposEnvasadoService.UpdateAsync(lTipoEnvasadoMap);
        }

        [HttpDelete("deleteTipoEnvasado")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteTipoEnvasado(int id)
        {
            return Ok(await _masterTiposEnvasadoService.DeleteTipoEnvasadoById(id));
        }
    }
}
