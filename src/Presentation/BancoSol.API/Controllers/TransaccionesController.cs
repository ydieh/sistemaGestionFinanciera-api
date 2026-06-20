using BancoSol.Application.DTOs;
using BancoSol.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BancoSol.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransaccionesController : ControllerBase
{
    private readonly IServicioTransacciones _servicioTransacciones;

    public TransaccionesController(IServicioTransacciones servicioTransacciones)
    {
        _servicioTransacciones = servicioTransacciones;
    }

    /// <summary>Registra una nueva transacción (ingreso =1).</summary>
    /// <remarks>
    /// Caso de Uso 1: Registro de Transacción.
    ///
    /// Solo se aceptan las monedas **BOB** (Bolivianos) y **USD** (Dólares).
    /// Cualquier otra moneda será rechazada con un 400 Bad Request.
    ///
    /// Ejemplo de solicitud:
    ///
    ///     POST /api/registraTransacciones
    ///     {
    ///         "monto": 5000,
    ///         "descripcion": "Sueldo de diciembre",
    ///         "fecha": "2025-12-01",
    ///         "origen": "sueldo",
    ///         "moneda": "BOB",
    ///         "tipo": 1,
    ///         "creadoPor": "juan.perez@email.com"
    ///     }
    ///
    /// **tipo**: 1 = Ingreso.
    /// </remarks>
    /// <response code="201">Transacción registrada exitosamente.</response>
    /// <response code="400">Datos inválidos (monto, moneda, etc.).</response>
    [HttpPost("registraTransaccion")]
    [ProducesResponseType(typeof(TransaccionRespuestaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegistrarTransaccion([FromBody] CrearTransaccionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var transaccionCreada = await _servicioTransacciones.RegistrarTransaccionAsync(dto);
        var respuesta = RespuestaApi<TransaccionRespuestaDto>.Exito(
            data: transaccionCreada,
            mensaje: "Ejecutado exitosamente",
            codigo: 201
        );
        return StatusCode(StatusCodes.Status201Created, respuesta);
        
    }
}
