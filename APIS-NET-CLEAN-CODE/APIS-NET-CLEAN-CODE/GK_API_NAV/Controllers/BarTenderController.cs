using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace GK_API_NAV.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarTenderController : ControllerBase
    {
            private readonly HttpClient _httpClient;
            private readonly ILogger<BarTenderController> _logger;

            // Se inyecta IHttpClientFactory y se obtiene el cliente configurado ("BartenderClient")
            public BarTenderController(ILogger<BarTenderController> pLogger, IHttpClientFactory httpClientFactory)
            {
                _logger = pLogger;
                _httpClient = httpClientFactory.CreateClient("BartenderClient");
            }

            /// <summary>
            /// Endpoint que recibe la solicitud de impresión y la reenvía al servicio de Bartender.
            /// </summary>
            /// <param name="request">Datos de la etiqueta a imprimir.</param>
            /// <returns>Resultado de la operación.</returns>
        //    [HttpPost("printlabel")]
        //    [Consumes("application/json")]
        //    public async Task<IActionResult> PrintLabel([FromBody] BarTenderEntityRequest request)
        //    {
        //    try
        //    {
        //        // Llamada al servicio Bartender
        //        var response = await _httpClient.PostAsJsonAsync("Integration/IntegraciónServiciowebPOST/Execute", request);
        //        var content = await response.Content.ReadAsStringAsync();

        //        // Parseamos la respuesta de Bartender
        //        var bartenderResponse = JsonSerializer.Deserialize<BartenderResponse>(content);

        //        // Aquí decides qué consideras un error (por ejemplo, mensajes con Level >= 3)
        //        if (bartenderResponse?.Messages != null &&
        //            bartenderResponse.Messages.Any(m => m.Level >= 3))
        //        {
        //            // Devuelve un 500 si se detecta un error en el contenido
        //            return StatusCode(500, new
        //            {
        //                message = "Error en la impresión",
        //                bartenderResponse
        //            });
        //        }

        //        return Ok(new { message = "Impresión enviada correctamente", bartenderResponse });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Ocurrió un error al comunicarse con el servicio de Bartender", detail = ex.Message });
        //    }
        //}
    }
    public class BartenderMessage
    {
        public string ActionName { get; set; }
        public int Level { get; set; }
        public string Text { get; set; }
    }

    public class BartenderResponse
    {
        public string Version { get; set; }
        public string Status { get; set; }
        public string WaitStatus { get; set; }
        public bool Validated { get; set; }
        public List<BartenderMessage> Messages { get; set; }
    }
}
