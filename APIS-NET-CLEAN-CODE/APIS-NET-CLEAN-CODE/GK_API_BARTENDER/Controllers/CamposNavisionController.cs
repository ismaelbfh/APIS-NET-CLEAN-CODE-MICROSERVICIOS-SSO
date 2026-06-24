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
    public class CamposNavisionController : ControllerBase
    {
        private readonly IServiceCamposNavision _camposNavisionService;
        private readonly IMapper _mapper;

        public CamposNavisionController(IServiceCamposNavision pCamposNavisionService, IMapper mapper)
        {
            _camposNavisionService = pCamposNavisionService;
            _mapper = mapper;
        }

        [HttpGet("getListCamposNavision")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<CamposNavisionDTO>>> GetListCamposNavision(int pageNumber = 1, int pageSize = 10, string filtro = "")
        {
            var lCamposNavisionResponse = await _camposNavisionService.GetCamposNavisionList(pageNumber, pageSize, filtro ?? "");
            if (lCamposNavisionResponse == null || lCamposNavisionResponse.Items == null || lCamposNavisionResponse.Items.Count == 0)
                return NoContent();
            return Ok(lCamposNavisionResponse);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<CamposNavisionDTO>> GetCampoNavisionById(int id)
        {
            var campoNavision = await _camposNavisionService.GetCampoNavisionById(id);
            return Ok(campoNavision);
        }

        [HttpPost("postCampoNavision")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> PostCampoNavision(MasterDataDTO pCampoNavisionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var lCampoNavisionMap = _mapper.Map<CamposNavision>(pCampoNavisionModel);
            await _camposNavisionService.AddAsync(lCampoNavisionMap);
            return CreatedAtAction(nameof(GetCampoNavisionById), new { id = lCampoNavisionMap.PK_IdCampoNavision }, _mapper.Map<CamposNavisionDTO>(lCampoNavisionMap));
        }

        [HttpPut("putCampoNavision")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> PutCampoNavision(MasterDataDTO pCampoNavisionModel)
        {
            var lCampoNavisionMap = _mapper.Map<CamposNavision>(pCampoNavisionModel);
            await _camposNavisionService.UpdateAsync(lCampoNavisionMap);
            return Ok(lCampoNavisionMap);
        }

        [HttpDelete("deleteCampoNavision")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteCampoNavision(int id)
        {
            var campoNavision = await _camposNavisionService.GetCampoNavisionById(id);
            await _camposNavisionService.DeleteAsync(_mapper.Map<CamposNavision>(campoNavision));
            return Ok();
        }
    }
}