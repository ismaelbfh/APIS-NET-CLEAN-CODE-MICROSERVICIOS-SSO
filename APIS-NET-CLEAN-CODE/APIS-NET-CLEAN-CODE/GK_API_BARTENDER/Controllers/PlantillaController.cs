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
    public class PlantillaController : ControllerBase
    {
        private readonly IServicePlantilla _srvPlantilla;
        private readonly IMapper _mapper;
        public PlantillaController(IServicePlantilla srvPlantilla, IMapper mapper)
        {
            _srvPlantilla = srvPlantilla;
            _mapper = mapper;
        }

        [HttpGet("getListPlantillas")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<PlantillaDTO>>> GetPlantillasList(
                                    [FromQuery] int pageNumber = 1,
                                    [FromQuery] int pageSize = 10,
                                    [FromQuery] string filtro = "")
        {
            var result = await _srvPlantilla.GetPlantillasList(pageNumber, pageSize, filtro);
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpPost("postPlantilla")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> PostPlantilla(PlantillaDTO pPlantillaModel)
        {
            // Asigna el usuario de modificación basado en el usuario autenticado.
            var usuarioClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Name);
            pPlantillaModel.UsuarioCreacion = usuarioClaim?.Value ?? "Sistema";
            await _srvPlantilla.AddPlantillaAsync(pPlantillaModel);
            return Ok();
        }

        [HttpPut("putPlantilla")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> PutPlantilla(PlantillaDTO pPlantillaModel)
        {
            // Asigna el usuario de modificación basado en el usuario autenticado.
            var usuarioClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Name);
            pPlantillaModel.UsuarioModificacion = usuarioClaim?.Value ?? "Sistema";
            await _srvPlantilla.UpdatePlantillaAsync(pPlantillaModel);
            return Ok();
        }

        [HttpDelete("deletePlantilla")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeletePlantilla(int id)
        {
            var plantilla = await _srvPlantilla.DeletePlantillaAsync(id);
            return Ok(plantilla);
        }
    }
}
