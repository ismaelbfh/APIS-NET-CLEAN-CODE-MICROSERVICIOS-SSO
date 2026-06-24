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
    public class MasterClientesController : ControllerBase
    {
        private readonly IServiceMasterClientes _masterClientesService;
        private readonly IMapper _mapper;
        public MasterClientesController(IServiceMasterClientes pMasterClientesService, IMapper mapper)
        {
            _masterClientesService = pMasterClientesService;
            _mapper = mapper;
        }

        [HttpGet("checkClienteEnUso/{idCliente}")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> CheckClienteEnUso(int idCliente)
        {
            var enUso = await _masterClientesService.EstaClienteEnUsoAsync(idCliente);
            return Ok(new { exists = enUso });
        }

        [HttpGet("getListClientes")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PaginatedResult<MasterDataDTO>>> GetClientesList(int pageNumber = 1, int pageSize = 10, string filtro = "")
        {
            var result = await _masterClientesService.GetClientesList(pageNumber, pageSize, filtro ?? "");
            if (result == null || result.Items == null || result.Items.Count == 0)
                return NoContent();
            return Ok(result);
        }

        [HttpPost("postCliente")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PostCliente(MasterDataDTO pClienteModel)
        {
            var lClienteMap = _mapper.Map<Cliente>(pClienteModel);
            await _masterClientesService.AddAsync(lClienteMap);
        }

        [HttpPut("putCliente")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task PutCliente(MasterDataDTO pClienteModel)
        {
            var lClienteMap = _mapper.Map<Cliente>(pClienteModel);
            await _masterClientesService.UpdateAsync(lClienteMap);
        }

        [HttpDelete("deleteCliente")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var lSeccion = await _masterClientesService.DeleteClienteById(id);
            return Ok(lSeccion);
        }
    }
}
