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

    /// <summary>Obtiene el historial completo de transacciones registradas.</summary>
    /// <remarks>
    /// Consulta de Historial Completo.
    ///
    /// Retorna todas las transacciones registradas en el sistema,
    /// ordenadas de la más reciente a la más antigua.
    ///
    /// Parámetro opcional **creadoPor**: filtra el historial para mostrar únicamente
    /// las transacciones registradas por ese usuario 
    ///
    /// Ejemplo de respuesta:
    ///
    ///     GET /api/Transacciones/obtenerTransacciones?creadoPor=juan.perez
    ///     {
    ///         "codigo": 200,
    ///         "mensaje": "Ejecutado exitosamente",
    ///         "data": [
    ///             {
    ///                 "id": 1,
    ///                 "monto": 5000,
    ///                 "descripcion": "Sueldo de diciembre",
    ///                 "fecha": "2025-12-01T00:00:00",
    ///                 "origen": "sueldo",
    ///                 "moneda": "BOB",
    ///                 "tipo": 1,
    ///                 "creadoPor": "juan.perez",
    ///                 "fechaCreacion": "2025-12-01T10:15:00Z"
    ///             }
    ///         ]
    ///     }
    ///
    /// Si no existen transacciones registradas, "data" se retorna como una lista vacía.
    /// </remarks>
    /// <response code="200">Listado de transacciones obtenido exitosamente.</response>
    [HttpGet("obtenerTransacciones")]
    [ProducesResponseType(typeof(RespuestaApi<IEnumerable<TransaccionRespuestaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTransacciones([FromQuery] string? creadoPor)
    {
        var transacciones = await _servicioTransacciones.ObtenerTodasAsync(creadoPor);
        var respuesta = RespuestaApi<IEnumerable<TransaccionRespuestaDto>>.Exito(
            data: transacciones,
            mensaje: "Ejecutado exitosamente",
            codigo: 200
        );
        return Ok(respuesta);
    }
}
