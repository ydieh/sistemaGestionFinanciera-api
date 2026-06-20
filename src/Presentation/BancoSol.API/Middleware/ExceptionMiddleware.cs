using BancoSol.Application.DTOs;
using BancoSol.Domain.Excepciones;
using System.Text.Json;

namespace BancoSol.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _siguiente;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate siguiente, ILogger<ExceptionMiddleware> logger)
    {
        _siguiente = siguiente;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext contexto)
    {
        try
        {
            await _siguiente(contexto);
        }
        catch (TransaccionNoEncontradaException ex)
        {
            await EscribirRespuestaAsync(contexto, StatusCodes.Status404NotFound, ex.Message);
        }
        catch (DomainException ex)
        {
            await EscribirRespuestaAsync(contexto, StatusCodes.Status400BadRequest, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado detectado en el pipeline de BancoSol");
            await EscribirRespuestaAsync(contexto, StatusCodes.Status500InternalServerError,
                "Ocurrió un error inesperado en el servidor. Por favor, intente más tarde.");
        }
    }

    private static async Task EscribirRespuestaAsync(HttpContext contexto, int statusCode, string mensaje)
    {
        contexto.Response.ContentType = "application/json";
        contexto.Response.StatusCode = statusCode;

        var respuestaError = RespuestaApi<object>.Error(mensaje, statusCode);

        await contexto.Response.WriteAsJsonAsync(respuestaError);
    }
}