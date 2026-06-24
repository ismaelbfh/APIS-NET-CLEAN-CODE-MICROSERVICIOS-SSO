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
    public class MasterSeccionesController : ControllerBase
    {
        private readonly IServiceMasterSecciones _masterSeccionesService;
        private readonly IMapper _mapper;
        public MasterSeccionesController(IServiceMasterSecciones pMasterSeccionesService, IMapper mapper)
        {
            _masterSeccionesService = pMasterSeccionesService;
            _mapper = mapper;
        }

        [HttpGet("checkSeccionEnUso/{idSeccion}")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> CheckSeccionEnUso(int idSeccion)
        {
            var enUso = await _masterSeccionesService.CheckSeccionEnUso(idSeccion);
            return Ok(new { exists = enUso });
        }

        [HttpGet("getListSecciones")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<MasterDataDTO>>> GetSeccionesList(int pageNumber = 1, int pageSize = 10, string filtro = "")
        {
            var result = await _masterSeccionesService.GetSeccionesList(pageNumber, pageSize, filtro ?? "");
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpPost("postSeccion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PostSeccion(MasterDataDTO pSeccionModel)
        {
            var lSeccionMap = _mapper.Map<Secciones>(pSeccionModel);
            await _masterSeccionesService.AddAsync(lSeccionMap);
        }

        [HttpPut("putSeccion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PutSeccion(MasterDataDTO pSeccionModel)
        {
            var lSeccionMap = _mapper.Map<Secciones>(pSeccionModel);
            await _masterSeccionesService.UpdateAsync(lSeccionMap);
        }

        [HttpDelete("deleteSeccion")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteSeccion(int id)
        {
            var lSeccion = await _masterSeccionesService.DeleteSeccionById(id);
            return Ok(lSeccion);
        }
    }
}
