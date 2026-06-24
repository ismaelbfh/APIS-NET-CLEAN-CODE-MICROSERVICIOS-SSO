using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Goikoa.Domain.GestionEtiquetas.DTOs.Responses;
using Goikoa.Domain.GestionEtiquetas.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Goikoa.Domain.GestionEtiquetas.Services
{
    public class ServiceBartender : IServiceBartender
    {
        private readonly HttpClient _httpClient;

        public ServiceBartender(IHttpClientFactory clientFactory)
        {
            _httpClient = clientFactory.CreateClient("BartenderClient");
        }

        public async Task<BartenderResponse> EnviarEtiquetaABartender(Dictionary<string, string> datosEtiqueta)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Integration/PostBartenderPrintEtiquetas/Execute", datosEtiqueta);
                var content = await response.Content.ReadAsStringAsync();

                Log.Information("Respuesta cruda de Bartender: {0}", content);

                var parsed = JsonSerializer.Deserialize<BartenderResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (parsed == null)
                {
                    Log.Error("Respuesta vacía o inválida de bartender");
                }

                return parsed ?? new BartenderResponse
                {
                    Messages = new List<BartenderMessage>
                    {
                        new BartenderMessage { ActionName = "Error", Level = 4, Text = "Respuesta vacía o inválida de Bartender" }
                    }
                };
            }
            catch (Exception ex)
            {
                Log.Error("Error al enviar etiqueta a Bartender {0}", ex.Message);
                return new BartenderResponse
                {
                    Messages = new List<BartenderMessage>
                    {
                        new BartenderMessage { ActionName = "Excepcion", Level = 4, Text = ex.Message }
                    }
                };
            }
        }
    }
}
