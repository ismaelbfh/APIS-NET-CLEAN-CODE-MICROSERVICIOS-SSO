using System.Text;
using GK_API_AUTH.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;
using System.Reflection;
using GK_API_AUTH.Swagger;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using AspNetCoreRateLimit;
using System;
using Microsoft.Extensions.Options;
using Goikoa.Domain.ApiAdmin.Entities;
using Goikoa.Domain.ApiAdmin.Interfaces;
using Goikoa.Domain.ApiAdmin.Services;
using Goikoa.Domain.ApiAdmin.Repositories;
using Goikoa.Domain.ApiAdmin.DAL.Context;
using Goikoa.Domain.ApiAdmin.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);

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

// 1. Configurar Data Protection (mismo setup que en la herramienta de encriptación)
builder.Services.AddDataProtection()
    .SetApplicationName("APP.DataProtection")
    .PersistKeysToFileSystem(new DirectoryInfo(@"C:\keys"));

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

// Desencriptar Jwt:Key
string? jwtKeyEncriptada = lConfig["Jwt:Key"];
if (!string.IsNullOrEmpty(jwtKeyEncriptada) && jwtKeyEncriptada.StartsWith("Encrypted:"))
{
    IDataProtectionProvider jwtProvider = DataProtectionProvider.Create(
        new DirectoryInfo(@"C:\keys\"),
        options => options.SetApplicationName("APP.GK.API"));

    IDataProtector jwtProtector = jwtProvider.CreateProtector("Jwt.Protector");
    string jwtKeyReal = jwtProtector.Unprotect(jwtKeyEncriptada["Encrypted:".Length..]);

    lConfig["Jwt:Key"] = jwtKeyReal;

    Log.Information("OK, se ha desencriptado correctamente la JWT Key.");
}

// 2. Obtener y desencriptar la cadena de conexión
string? lConexionEncriptadaAPI = lConfig.GetConnectionString("ConnectionStringApi");
string? lConexionEncriptadaNavision = lConfig.GetConnectionString("ConnectionStringNavision");

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

if (!string.IsNullOrEmpty(lConexionEncriptadaNavision) && lConexionEncriptadaNavision.StartsWith("Encrypted:"))
{
    // Crear el proveedor de desencriptación fuera del DI container
    IDataProtectionProvider lTempProvider = DataProtectionProvider.Create(
        new DirectoryInfo(@"C:\keys\"),
        options => options.SetApplicationName("APP.DataProtection"));

    IDataProtector lProtector = lTempProvider.CreateProtector("ConnectionStrings.Protector");
    string lConexionReal = lProtector.Unprotect(lConexionEncriptadaNavision["Encrypted:".Length..]);

    // Actualizar la configuración con el valor desencriptado
    lConfig["ConnectionStrings:ConnectionStringNavision"] = lConexionReal;
    Serilog.Log.Information("OK, se ha desencriptado correctamente la cadena de conexion de Navision.");
}

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

builder.Services.AddAutoMapper(typeof(MappingUserProfile));
builder.Services.AddAutoMapper(typeof(MappingUserEntitiesProfile));
builder.Services.AddAutoMapper(typeof(MappingRefreshTokenEntitiesProfile));
builder.Services.AddAutoMapper(typeof(MappingLogsEntitiesProfile));
builder.Services.AddAutoMapper(typeof(MappingLogProfile));

string connectionStringApi = lConfig.GetConnectionString("ConnectionStringApi");

// Añadir DbContext a los servicios, configurando la cadena de conexión
builder.Services.AddDbContext<ApiAdminContext>(options =>
    options.UseSqlServer(connectionStringApi));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<ILogService, LogService>();

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

// Agregar cache en memoria (requerido para el rate limiting)
builder.Services.AddMemoryCache();

// Configurar las opciones de rate limiting leyendo de appsettings.json
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));

// Registrar el rate limiting en memoria
builder.Services.AddInMemoryRateLimiting();

// Registrar la configuración global para el rate limiting
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowReactApp");

// Agregar el middleware de rate limiting antes del routing, si es desarrollo lo desactivamos
if (!builder.Environment.IsDevelopment())
{
    app.UseIpRateLimiting();
}

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

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    IServiceProvider lServices = scope.ServiceProvider;

    IUserRepository lUserRepository = lServices.GetRequiredService<IUserRepository>();
    IRefreshTokenRepository lRefreshTokenRepository = lServices.GetRequiredService<IRefreshTokenRepository>();
    IConfiguration lConfiguration = lServices.GetRequiredService<IConfiguration>();

    await SeedAdminUser(lUserRepository, lRefreshTokenRepository, lConfiguration);
}

app.Run();

async Task SeedAdminUser(IUserRepository pUserRepository, IRefreshTokenRepository pRefreshTokenRepository, IConfiguration pConfiguration)
{
    // Lee las credenciales admin desde la configuración
    string lAdminUsername = pConfiguration["AdminUser:Username"];
    string lAdminPassword = pConfiguration["AdminUser:Password"];
    string lAdminUsernameDecrypt = string.Empty;
    string lAdminPasswordDecrypt = string.Empty;

    //desencriptar y comprobar username y password
    if (!string.IsNullOrEmpty(lAdminUsername) && lAdminUsername.StartsWith("Encrypted:"))
    {
        // Crear el proveedor de desencriptación fuera del DI container
        IDataProtectionProvider lTempProvider = DataProtectionProvider.Create(
            new DirectoryInfo(@"C:\keys\"),
            options => options.SetApplicationName("APP.DataProtection"));

        IDataProtector lProtector = lTempProvider.CreateProtector("AdminUser.Protector");
        lAdminUsernameDecrypt = lProtector.Unprotect(lAdminUsername["Encrypted:".Length..]);

        // Actualizar la configuración con el valor desencriptado
        pConfiguration["AdminUser:Username"] = lAdminUsernameDecrypt;
        Serilog.Log.Information("OK, se ha desencriptado correctamente el nombre del usuario admin.");
    }

    if (!string.IsNullOrEmpty(lAdminPassword) && lAdminPassword.StartsWith("Encrypted:"))
    {
        // Crear el proveedor de desencriptación fuera del DI container
        IDataProtectionProvider lTempProvider = DataProtectionProvider.Create(
            new DirectoryInfo(@"C:\keys\"),
            options => options.SetApplicationName("APP.DataProtection"));

        IDataProtector lProtector = lTempProvider.CreateProtector("AdminUser.Protector");
        lAdminPasswordDecrypt = lProtector.Unprotect(lAdminPassword["Encrypted:".Length..]);

        // Actualizar la configuración con el valor desencriptado
        pConfiguration["AdminUser:Password"] = lAdminPasswordDecrypt;
        Serilog.Log.Information("OK, se ha desencriptado correctamente la contraseña del usuario admin.");
    }

    // Verificar si ya existe un usuario administrador
    UserEntity? lAdminUser = await pUserRepository.GetUserByNameAsync(lAdminUsernameDecrypt);
    if (lAdminUser == null)
    {
        try
        {
            UserEntity lNewAdminUser = UserEntity.CreateUser(lAdminUsernameDecrypt, lAdminPasswordDecrypt, 1);

            lNewAdminUser = await pUserRepository.AddAsync(lNewAdminUser);

            Serilog.Log.Information("OK, se ha creado con éxito el usuario admin para este entorno.");

            int refreshTokenExpirationDays = int.Parse(pConfiguration["Jwt:RefreshTokenExpirationDays"]);

            // Usamos el método de fábrica de la entidad para crear el refresh token
            RefreshTokenEntity lRefreshToken = RefreshTokenEntity.Create(lNewAdminUser.PkIdUser, refreshTokenExpirationDays);

            await pRefreshTokenRepository.AddAsync(lRefreshToken);
        }
        catch (Exception ex)
        {
            Serilog.Log.Error("Error al crear usuario admin inesperado, exception: {ExceptionM}", ex.Message);
        }
    }
}
public partial class ProgramApiAuth { }