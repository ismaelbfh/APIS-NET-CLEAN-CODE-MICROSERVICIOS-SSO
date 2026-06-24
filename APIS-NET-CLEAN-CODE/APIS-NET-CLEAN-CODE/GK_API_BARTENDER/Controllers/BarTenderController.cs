using System.Text.Json;
using Goikoa.Domain.GestionEtiquetas.DTOs.Responses;
using Goikoa.Domain.GestionEtiquetas.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GK_API_BARTENDER.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BartenderController : ControllerBase
    {
        private readonly IServiceBartender _bartenderService;

        public BartenderController(IServiceBartender bartenderService)
        {
            _bartenderService = bartenderService;
        }

        [HttpPost("printLabel")]
        [Authorize(Roles = "Admin,Editor,Viewer")]
        public async Task<ActionResult<PrintLabelResultDTO>> PrintLabel([FromBody] Dictionary<string, string> datos)
        {
            BartenderResponse result = await _bartenderService.EnviarEtiquetaABartender(datos);

            if (result.Messages != null && result.Messages.Any(m => m.Level >= 4))
            {
                return StatusCode(500, new PrintLabelResultDTO
                {
                    message = "Error en la impresión",
                    bartenderResponse = result
                });
            }

            if (result.Messages != null && result.Messages.Any(m => m.Level == 3))
            {
                return Ok(new PrintLabelResultDTO
                {
                    message = "Impresión realizada con advertencias",
                    bartenderResponse = result
                });
            }

            return Ok(new PrintLabelResultDTO
            {
                message = "Impresión enviada correctamente",
                bartenderResponse = result
            });
        }
    }

}
