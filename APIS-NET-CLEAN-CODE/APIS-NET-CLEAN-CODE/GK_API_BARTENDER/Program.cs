using System.Text;
using GK_API_NAV.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;
using System.Reflection;
using GK_API_NAV.Swagger;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System;
using Microsoft.Extensions.Options;
using Goikoa.Domain.GestionEtiquetas.Interfaces;
using Goikoa.Domain.GestionEtiquetas.Services;
using Goikoa.Domain.GestionEtiquetas.DAL.Context;
using Goikoa.Domain.GestionEtiquetas.Mappers;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager lConfig = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(lConfig["UrlAppReact"]) // Agrega la URL de tu aplicación React
              .AllowAnyMethod() // Permite todos los métodos HTTP (GET, POST, etc.)
              .AllowAnyHeader() // Permite todos los encabezados
              .AllowCredentials(); // Permite el uso de cookies o credenciales
    });
});

// Configurar la respuesta de error de validación personalizada
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    // Desactiva el comportamiento automático de validación para que no se retorne el ProblemDetails por defecto
    options.SuppressModelStateInvalidFilter = true;
});

// Add services to the container.

builder.Services.AddControllers(
                        opt =>
                        {
                            //Custom filters, if needed
                            opt.Filters.Add(typeof(ValidateModelAttribute));
                            opt.Filters.Add(new ProducesAttribute("application/json"));
                        }).AddNewtonsoftJson(options =>
                        {
                            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        });


builder.Services.AddHttpClient("BartenderClient", client =>
{
    client.BaseAddress = new Uri("http://192.168.44.126");
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddScoped<IServiceBartender, ServiceBartender>();

// 1. Configurar Data Protection (mismo setup que en la herramienta de encriptación)
builder.Services.AddDataProtection()
    .SetApplicationName("APP.DataProtection")
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys"));

// Desencriptar Jwt:Key
string? jwtKeyEncriptada = lConfig["Jwt:Key"];
if (!string.IsNullOrEmpty(jwtKeyEncriptada) && jwtKeyEncriptada.StartsWith("Encrypted:"))
{
    IDataProtectionProvider jwtProvider = DataProtectionProvider.Create(
        new DirectoryInfo(@"C:\keys\"),
        options => options.SetApplicationName("APP.DataProtection"));

    IDataProtector jwtProtector = jwtProvider.CreateProtector("Jwt.Protector");
    string jwtKeyReal = jwtProtector.Unprotect(jwtKeyEncriptada["Encrypted:".Length..]);

    lConfig["Jwt:Key"] = jwtKeyReal;

    Log.Information("OK, se ha desencriptado correctamente la JWT Key.");
}

// 2. Obtener y desencriptar la cadena de conexión

string? lConexionEncriptadaAPI = lConfig.GetConnectionString("ConnectionStringApi");
string? lConexionEncriptadaEtiquetas = lConfig.GetConnectionString("ConnectionStringEtiquetas");

if (!string.IsNullOrEmpty(lConexionEncriptadaAPI) && lConexionEncriptadaAPI.StartsWith("Encrypted:"))
{
    // Crear el proveedor de desencriptación fuera del DI container
    IDataProtectionProvider lTempProvider = DataProtectionProvider.Create(
        new DirectoryInfo(@"C:\keys\"),
        options => options.SetApplicationName("APP.DataProtection"));

    IDataProtector lProtector = lTempProvider.CreateProtector("ConnectionStrings.Protector");
    string lConexionReal = lProtector.Unprotect(lConexionEncriptadaAPI["Encrypted:".Length..]);

    // Actualizar la configuración con el valor desencriptado
    lConfig["ConnectionStrings:ConnectionStringApi"] = lConexionReal;

    Serilog.Log.Information("OK, se ha desencriptado correctamente la cadena de conexion de la API.");

}

if (!string.IsNullOrEmpty(lConexionEncriptadaEtiquetas) && lConexionEncriptadaEtiquetas.StartsWith("Encrypted:"))
{
    // Crear el proveedor de desencriptación fuera del DI container
    IDataProtectionProvider lTempProvider = DataProtectionProvider.Create(
        new DirectoryInfo(@"C:\keys\"),
        options => options.SetApplicationName("APP.DataProtection"));

    IDataProtector lProtector = lTempProvider.CreateProtector("ConnectionStrings.Protector");
    string lConexionReal = lProtector.Unprotect(lConexionEncriptadaEtiquetas["Encrypted:".Length..]);

    // Actualizar la configuración con el valor desencriptado
    lConfig["ConnectionStrings:ConnectionStringEtiquetas"] = lConexionReal;
    Serilog.Log.Information("OK, se ha desencriptado correctamente la cadena de conexion de Etiquetas.");
}

// Obtener el entorno actual
//var environment = builder.Environment.EnvironmentName; // Development, Preproduction, Production

// Leer todas las URLs del appsettings.json
//var baseUrls = builder.Configuration.GetSection("Environments").Get<Dictionary<string, string>>();

// Seleccionar la URL según el entorno actual
//var baseUrl = baseUrls.ContainsKey(environment) ? baseUrls[environment] : "https://localhost:5001";

// Configurar Kestrel para usar HTTPS
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps()); // HTTPS
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuración del sink para SQL Server
var columnOptions = new ColumnOptions();
// Opcional: puedes personalizar las columnas, por ejemplo, para registrar propiedades adicionales
columnOptions.Store.Remove(StandardColumn.Properties); // Quitar columna Properties si no la necesitas

Serilog.Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    // Configuración del sink a SQL Server:
    .WriteTo.MSSqlServer(
         connectionString: builder.Configuration.GetConnectionString("ConnectionStringApi"),
         sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = false },
         columnOptions: columnOptions,
         restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
    .CreateLogger();

// Reemplaza el logging predeterminado por Serilog
builder.Host.UseSerilog();

builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
});

builder.Services.AddAutoMapper(typeof(MastersProfile));

string connectionStringApi = lConfig.GetConnectionString("ConnectionStringApi");
string connectionStringEtiquetas = lConfig.GetConnectionString("ConnectionStringEtiquetas");

// Añadir DbContext a los servicios, configurando la cadena de conexión

builder.Services.AddDbContext<BDGestionEtiquetasContext>(options =>
    options.UseSqlServer(connectionStringEtiquetas));

builder.Services.AddScoped<IServiceMasterCampo, ServiceMasterCampo>();
builder.Services.AddScoped<IServiceMasterTiposCampo, ServiceMasterTiposCampo>();
builder.Services.AddScoped<IServiceMasterTiposEtiqueta, ServiceMasterTiposEtiqueta>();
builder.Services.AddScoped<IServiceMasterTiposEnvasado, ServiceMasterTiposEnvasado>();
builder.Services.AddScoped<IServiceMasterTiposConservacion, ServiceMasterTiposConservacion>();
builder.Services.AddScoped<IServiceMasterSecciones, ServiceMasterSecciones>();
builder.Services.AddScoped<IServiceMasterClientes, ServiceMasterClientes>();
builder.Services.AddScoped<IServicePlantilla, ServicePlantilla>();
builder.Services.AddScoped<IServiceEtiqueta, ServiceEtiquetas>();
builder.Services.AddScoped<IServiceMasterTiposCodigoBarra, ServiceMasterTiposCodigoBarra>();
builder.Services.AddScoped<IServiceMasterLabels, ServiceMasterLabelsMaster>();
builder.Services.AddScoped<IServiceMasterTiposLabel, ServiceMasterTiposLabel>();
builder.Services.AddScoped<IServiceMasterIdiomasLabel, ServiceMasterIdiomasLabel>();
builder.Services.AddScoped<IServiceHistoricoImpresion, ServiceHistoricoImpresion>();
builder.Services.AddScoped<IServiceCamposNavision, ServiceCamposNavision>();
builder.Services.AddScoped<IServiceMasterUbicaciones, ServiceMasterUbicaciones>();
builder.Services.AddScoped<IServiceCamposWSBartender, ServiceCamposWSBartender>();



// Configuración de autenticación con JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]))
        };
    });

builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<SwaggerDefaultValues>();
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Bearer",
        BearerFormat = "JWT",
        Scheme = "bearer",
        Type = SecuritySchemeType.Http
    });
    options.UseAllOfToExtendReferenceSchemas();
    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                                       {
                                         new OpenApiSecurityScheme
                                         {
                                           Reference = new OpenApiReference
                                           {
                                             Type = ReferenceType.SecurityScheme,
                                             Id = "Bearer"
                                           }
                                          },
                                          new string[] { }
                                        }
                                  });

    //1-Get all the assemblies of the project to add the related XML Comments
    Assembly currentAssembly = Assembly.GetExecutingAssembly();
    AssemblyName[] referencedAssemblies = currentAssembly.GetReferencedAssemblies();
    IEnumerable<AssemblyName> allAssemblies = null;

    if (referencedAssemblies != null && referencedAssemblies.Any())
        allAssemblies = referencedAssemblies.Union(new AssemblyName[] { currentAssembly.GetName() });
    else
        allAssemblies = new AssemblyName[] { currentAssembly.GetName() };

    IEnumerable<string> xmlDocs = allAssemblies
            .Select(a => Path.Combine(Path.GetDirectoryName(currentAssembly.Location)!, $"{a.Name}.xml"))
            .Where(f => File.Exists(f));

    //2-Add the path to the XML comments for the assemblies having enabled the doc generation
    if (xmlDocs != null && xmlDocs.Any())
    {
        foreach (string? item in xmlDocs)
        {
            options.IncludeXmlComments(item);
        }
    }
});


var app = builder.Build();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowReactApp");
app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerPathFeature != null)
        {
            var exception = exceptionHandlerPathFeature.Error;

            // Determinar el código de estado HTTP basado en el tipo de excepción
            HttpStatusCode statusCode = exception switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,
                ApplicationException => HttpStatusCode.BadRequest,
                ArgumentNullException or ArgumentException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                _ => HttpStatusCode.InternalServerError // Para cualquier otra excepción
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var errorResponse = new
            {
                message = exception.Message, // Asegura que devuelve tu mensaje personalizado
                source = exception.Source,
                exceptionType = exception.GetType().FullName,
                statusCode = (int)statusCode
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
        }
    });
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

public partial class ProgramBartender { }