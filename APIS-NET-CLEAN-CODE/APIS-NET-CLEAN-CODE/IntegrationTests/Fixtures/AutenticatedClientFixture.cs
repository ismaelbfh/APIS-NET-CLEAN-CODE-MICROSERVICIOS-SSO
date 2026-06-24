using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using IntegrationTests.ApiAdmin.Factories;
using Goikoa.Domain.ApiAdmin.DTOs.Requests;
using Goikoa.Domain.ApiAdmin.DTOs.Responses;

namespace IntegrationTests.ApiAdmin.Fixtures
{
    public class AuthenticatedClientFixture : IAsyncLifetime
    {
        public HttpClient ClientNegocio { get; private set; }

        private readonly AuthApiFactory _authFactory;
        private readonly NavisionApiFactory _negocioFactory;

        public AuthenticatedClientFixture()
        {
            _authFactory = new AuthApiFactory();
            _negocioFactory = new NavisionApiFactory();
        }

        public async Task InitializeAsync()
        {
            // Crear cliente de Auth y hacer login
            var authClient = _authFactory.CreateClient();

            var loginRequest = new UserRequest
            {
                Username = "gk_camera_user_wpf",
                Password = "1234567"
            };

            var loginResponse = await authClient.PostAsJsonAsync("/api/auth/login", loginRequest);
            loginResponse.EnsureSuccessStatusCode();

            var token = await loginResponse.Content.ReadFromJsonAsync<TokenResponseDTO>();

            // Crear cliente de Negocio con token
            ClientNegocio = _negocioFactory.CreateClient();

            if (!string.IsNullOrWhiteSpace(token?.AccessToken))
            {
                ClientNegocio.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token.AccessToken);
            }
        }

        public Task DisposeAsync() => Task.CompletedTask;
    }
}
