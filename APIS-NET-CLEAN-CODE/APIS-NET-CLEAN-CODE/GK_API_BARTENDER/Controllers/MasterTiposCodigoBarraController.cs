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
    public class MasterTiposCodigoBarraController : ControllerBase
    {
        private readonly IServiceMasterTiposCodigoBarra _masterTiposCodigoBarraService;
        private readonly IMapper _mapper;
        public MasterTiposCodigoBarraController(IServiceMasterTiposCodigoBarra pMasterTiposCodigoBarraService, IMapper mapper)
        {
            _masterTiposCodigoBarraService = pMasterTiposCodigoBarraService;
            _mapper = mapper;
        }

        [HttpGet("getSelectDropdownCodificacionList")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> GetSelectDropdownCodificacionAsync()
        {
            var lista = await _masterTiposCodigoBarraService.GetSelectDropdownCodificacionAsync();
            return Ok(lista);
        }

        [HttpGet("checkTipoCodigoBarraEnUso/{idTipoCodigoBarra}")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> CheckTipoEtiquetaEnUso(int idTipoCodigoBarra)
        {
            var estaEnUso = await _masterTiposCodigoBarraService.ExisteTipoCodigoBarraEnEtiquetas(idTipoCodigoBarra);
            return Ok(new { exists = estaEnUso });
        }

        [HttpGet("getListTiposCodigosBarra")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<MasterDataDTO>>> GetTiposCodigosBarraList(int pageNumber = 1, int pageSize = 10, string filtro = "")
        {
            var result = await _masterTiposCodigoBarraService.GetTiposCodigoBarraList(pageNumber, pageSize, filtro ?? "");
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpPost("postTipoCodigoBarra")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PostTipoCodigoBarra(MasterDataDTO pTipoCodigoBarraModel)
        {
            var lTipoCodigoBarraMap = _mapper.Map<TiposCodigoBarra>(pTipoCodigoBarraModel);
            await _masterTiposCodigoBarraService.AddAsync(lTipoCodigoBarraMap);
        }

        [HttpPut("putTipoCodigoBarra")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PutTipoCodigoBarra(MasterDataDTO pTipoCodigoBarraModel)
        {
            var lTipoCodigoBarraMap = _mapper.Map<TiposCodigoBarra>(pTipoCodigoBarraModel);
            await _masterTiposCodigoBarraService.UpdateAsync(lTipoCodigoBarraMap);
        }

        [HttpDelete("deleteTipoCodigoBarra")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteTipoCodigoBarra(int id)
        {
            var tipoCodigoBarra = await _masterTiposCodigoBarraService.DeleteTipoCodigoBarraById(id);
            return Ok(tipoCodigoBarra);
        }
    }
}
