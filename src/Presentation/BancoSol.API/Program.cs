using BancoSol.API.Middleware;
using BancoSol.Application.Interfaces;
using BancoSol.Application.Servicios;
using BancoSol.Domain.Interfaces;
using BancoSol.Infrastructure.Datos;
using BancoSol.Infrastructure.Repositorios;
using BancoSol.Infrastructure.ServiciosExternos;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var cadenaConexion = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(cadenaConexion))
{
    var host = builder.Configuration["MYSQLHOST"];
    var puerto = builder.Configuration["MYSQLPORT"] ?? "3306";
    var baseDatos = builder.Configuration["MYSQLDATABASE"];
    var usuario = builder.Configuration["MYSQLUSER"];
    var clave = builder.Configuration["MYSQLPASSWORD"];

    cadenaConexion = $"Server={host};Port={puerto};Database={baseDatos};User={usuario};Password={clave};";
    
}

if (string.IsNullOrWhiteSpace(cadenaConexion))
{
    throw new InvalidOperationException(
        "Cadena de conexión no configurada. Configure 'ConnectionStrings:DefaultConnection' o las variables de entorno MYSQLHOST, MYSQLDATABASE, MYSQLUSER y MYSQLPASSWORD.");
}

builder.Services.AddDbContext<BancoSolDbContext>(opciones =>
    opciones.UseMySql(cadenaConexion, ServerVersion.AutoDetect(cadenaConexion)));

builder.Services.AddScoped<IRepositorioTransacciones, RepositorioTransacciones>();

builder.Services.AddScoped<IServicioTransacciones, ServicioTransacciones>();
builder.Services.AddScoped<IServicioReportes, ServicioReportes>();

builder.Services.AddHttpClient<IServicioTipoCambio, ServicioTipoCambioHexaRate>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BancoSol API",
        Version = "v1",
        Description = "API REST para gestión de finanzas personales. " +
                       "Permite registrar transacciones (ingresos ) en Bolivianos (BOB) " +
                       "y Dólares (USD), y obtener reportes consolidados.",
        Contact = new OpenApiContact { Name = "BancoSol Dev Team" }
    });

    opciones.UseInlineDefinitionsForEnums();

    var archivoXml = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var rutaXml = Path.Combine(AppContext.BaseDirectory, archivoXml);
    if (File.Exists(rutaXml))
        opciones.IncludeXmlComments(rutaXml);
});

builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(politica =>
        politica.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BancoSolDbContext>();
    db.Database.Migrate();
}
app.UseSwagger();
app.UseSwaggerUI(opciones =>
{
    opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "BancoSol API v1");
    opciones.RoutePrefix = "swagger"; 
    opciones.DocumentTitle = "BancoSol API - Documentación";
});

app.UseCors();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
public partial class Program { }
