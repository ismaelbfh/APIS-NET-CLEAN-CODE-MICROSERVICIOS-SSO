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
    public class EtiquetaController : ControllerBase
    {
        private readonly IServiceEtiqueta _etiquetaService;
        private readonly IMapper _mapper;
        public EtiquetaController(IServiceEtiqueta srvEtiqueta, IMapper mapper)
        {
            _etiquetaService = srvEtiqueta;
            _mapper = mapper;
        }

        [HttpGet("getListEtiquetas")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<EtiquetaDTO>>> GetListEtiquetas(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string filtro = "",
        [FromQuery] int? plantillaId = null)
        {
            var result = await _etiquetaService.GetEtiquetasList(pageNumber, pageSize, filtro ?? "", plantillaId);
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpGet("getEtiquetaPorCodigoProducto")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<Etiqueta>> GetEtiquetaPorCodigoProducto([FromQuery] string codigoProducto)
        {
            var etiqueta = await _etiquetaService.GetEtiquetaPorCodigoProductoAsync(codigoProducto);
            return Ok(etiqueta);
        }

        [HttpPost("postEtiqueta")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> PostEtiqueta(EtiquetaDTO etiquetaDto)
        {
            var usuarioClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Name);
            etiquetaDto.Usuario = usuarioClaim?.Value ?? "Sistema";
            await _etiquetaService.AddEtiquetaAsync(etiquetaDto);
            return Ok();
        }

        [HttpPut("putEtiqueta")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> PutEtiqueta(EtiquetaDTO etiquetaDto)
        {
            var usuarioClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Name);
            etiquetaDto.Usuario = usuarioClaim?.Value ?? "Sistema";
            await _etiquetaService.UpdateEtiquetaAsync(etiquetaDto);
            return Ok();
        }

        [HttpDelete("deleteEtiqueta")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteEtiqueta(int id)
        {
            var et = await _etiquetaService.DeleteEtiquetaAsync(id);
            return Ok(et);
        }
    }
}
