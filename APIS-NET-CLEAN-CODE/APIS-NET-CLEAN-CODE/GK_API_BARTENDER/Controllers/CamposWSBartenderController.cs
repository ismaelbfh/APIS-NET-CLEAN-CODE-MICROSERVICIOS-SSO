using Goikoa.Domain.GestionEtiquetas.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CamposWSBartenderController : ControllerBase
{
    private readonly IServiceCamposWSBartender _service;

    public CamposWSBartenderController(IServiceCamposWSBartender service)
    {
        _service = service;
    }

    [HttpGet("getTodosCampos")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetTodosCampos([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetTodosCamposPaginadoAsync(page, pageSize);
        return Ok(result);
    }

    [HttpGet("getCamposFaltantes")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetCamposFaltantes()
    {
        var campos = await _service.GetCamposFaltantesAsync();
        return Ok(campos);
    }

    [HttpPost("registrarCampos")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RegistrarCampos([FromBody] List<string> nombres)
    {
        var añadidos = await _service.RegistrarCamposAsync(nombres);
        return Ok(new
        {
            Mensaje = "Campos añadidos correctamente.",
            Añadidos = añadidos
        });
    }
}
