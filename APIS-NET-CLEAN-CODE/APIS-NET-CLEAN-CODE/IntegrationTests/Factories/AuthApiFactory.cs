using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace IntegrationTests.ApiAdmin.Factories
{
    public class AuthApiFactory : WebApplicationFactory<ProgramApiAuth>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            return base.CreateHost(builder);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // En este caso, no eliminamos ni creamos la base de datos,
                // ni ejecutamos ningún seed. Se usará la base de datos configurada en appsettings.
                // O bien, puedes dejar aquí código para limpiar o preparar el entorno si fuera necesario.
            });
        }
    }
}
