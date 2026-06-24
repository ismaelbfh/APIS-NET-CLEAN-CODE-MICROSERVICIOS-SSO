using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
using Goikoa.Domain.Navision.Producccion.Entities;
using Goikoa.Domain.Navision.Producccion.DTOs.Responses;
using IntegrationTests.ApiAdmin.Fixtures;

namespace IntegrationTests.ApiAdmin
{
    public class TestOrdenesProduccion : IClassFixture<AuthenticatedClientFixture>
    {
        private readonly HttpClient _client;

        public TestOrdenesProduccion(AuthenticatedClientFixture fixture)
        {
            _client = fixture.ClientNegocio;
        }

        [Fact]
        public async Task GetOrdenesProduccion_ReturnsData()
        {
            // Arrange: Se hardcodean los parámetros de la consulta
            var url = "/api/OrdenesNavision/getOrdenesProduccion?pLine=04MGAS&pTipoDestino=2&pStartingDate=2021-05-04&pPageNumber=1&pPageSize=50";

            // Act: Se llama al endpoint
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // Se deserializa la respuesta
            PaginatedResult<GKOrdenProdDTO>? ordenes = JsonSerializer.Deserialize<PaginatedResult<GKOrdenProdDTO>?>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //Assert: comprobamos que tienen ordenes sino falla el test
            Assert.True(ordenes != null && ordenes.Items != null && ordenes.Items.Any(), "No hay órdenes para mostrar, cambie los parametros y vuelva a probar");
        }

        [Fact]
        public async Task GetDatosOrdenProduccion_ReturnsData()
        {
            // Arrange
            var url = $"/api/OrdenesNavision/getDatosOrdenProduccion?pOPFabricacion=OL20-00338";

            // Act
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // Se deserializa la respuesta
            var orden = JsonSerializer.Deserialize<GKInfoOrdenCamaraDTO>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.True(orden != null, "Orden no encontrada, pruebe otra.");
            Assert.True(orden.OPFabricacion.Equals("OL20-00338"), "Orden obtenida no coincide.");
        }

        [Fact]
        public async Task GetCodigoBarras_ReturnsData()
        {
            // Arrange
            var url = $"/api/OrdenesNavision/getCodigoBarras?pNumProduct=61975&pTipoCodigoBarras=EAN13";

            // Act
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            // Se remueven comillas si las hubiera
            string? codigoBarras = content.Trim('\"');

            // Assert
            Assert.True(!string.IsNullOrEmpty(codigoBarras), "No se encontró codigo de barras para ese producto, pruebe con otro.");
        }
    }
}
