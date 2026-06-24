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
    public class HistoricoImpresionController : ControllerBase
    {
        private readonly IServiceHistoricoImpresion _historicoService;
        private readonly IMapper _mapper;
        public HistoricoImpresionController(IServiceHistoricoImpresion srvHistorico, IMapper mapper)
        {
            _historicoService = srvHistorico;
            _mapper = mapper;
        }

        [HttpGet("getListHistoricoImpresion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<HistoricoImpresionDTO>>> GetListHistoricoImpresion(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string filtro = "")
        {
            var result = await _historicoService.GetHistoricosList(pageNumber, pageSize, filtro);
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }
        [HttpPost("postHistoricoImpresion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> PostHistoricoImpresion(HistoricoImpresionDTO historicoImpresionDto)
        {
            await _historicoService.AddHistoricoImpresionAsync(historicoImpresionDto);
            return Ok();
        }

    }
}
