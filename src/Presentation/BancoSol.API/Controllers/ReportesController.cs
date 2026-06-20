using BancoSol.Application.DTOs;
using BancoSol.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BancoSol.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ReportesController : ControllerBase
{
    private readonly IServicioReportes _servicioReportes;

    public ReportesController(IServicioReportes servicioReportes)
    {
        _servicioReportes = servicioReportes;
    }

    /// <summary>Consulta el tipo de cambio USD/BOB vigente.</summary>
    /// <remarks>
    /// Caso de Uso 4: Obtención de Información de Cambio de Moneda.
    ///
    /// Consulta la tasa de conversión actual desde HexaRate API
    /// (https://hexarate.paikama.co/api/rates/USD/BOB/latest). Esta misma información es
    /// la que usa internamente el Caso de Uso 5 para convertir montos entre monedas;
    /// se expone también como endpoint propio para poder verificarla de forma independiente.
    ///
    /// Ejemplo de respuesta:
    ///
    ///     GET /api/Reportes/tipoCambio
    ///     {
    ///         "codigo": 200,
    ///         "mensaje": "Ejecutado exitosamente",
    ///         "data": {
    ///             "origen": "USD",
    ///             "destino": "BOB",
    ///             "tasa": 6.92,
    ///             "consultadoEn": "2025-12-01T10:00:00Z"
    ///         }
    ///     }
    /// </remarks>
    /// <response code="200">Tipo de cambio obtenido exitosamente.</response>
    /// <response code="500">No se pudo consultar la fuente externa (HexaRate).</response>
    [HttpGet("tipoCambio")]
    [ProducesResponseType(typeof(RespuestaApi<TipoCambioDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ObtenerTipoCambioActual()
    {
        var tipoCambio = await _servicioReportes.ObtenerTipoCambioActualAsync();
        var respuesta = RespuestaApi<TipoCambioDto>.Exito(
            data: tipoCambio,
            mensaje: "Ejecutado exitosamente",
            codigo: 200
        );
        return Ok(respuesta);
    }

    /// <summary>Obtiene el balance consolidado de ingresos en un período y moneda específicos.</summary>
    /// <remarks>
    /// Caso de Uso 5: Reporte de Balance Consolidado.
    ///
    /// Suma todos los ingresos registrados dentro del rango de fechas, convirtiendo a la
    /// moneda solicitada los que estén en una moneda diferente, usando el tipo de cambio vigente.
    ///
    /// Parámetro opcional **creadoPor**: limita el cálculo a los ingresos de ese usuario.
    ///
    /// Ejemplo de solicitud:
    ///
    ///     GET /api/Reportes/balance?fechaInicio=2025-12-01&amp;fechaFin=2025-12-31&amp;moneda=BOB
    ///
    /// Ejemplo de respuesta (según el Ejemplo 1 del documento de requerimientos):
    ///
    ///     {
    ///         "codigo": 200,
    ///         "mensaje": "Ejecutado exitosamente",
    ///         "data": {
    ///             "fechaInicio": "2025-12-01T00:00:00",
    ///             "fechaFin": "2025-12-31T00:00:00",
    ///             "moneda": "BOB",
    ///             "balanceTotal": 5692.00,
    ///             "cantidadIngresosConsiderados": 3,
    ///             "tipoCambioUsado": 6.92
    ///         }
    ///     }
    /// </remarks>
    /// <response code="200">Balance calculado exitosamente.</response>
    /// <response code="400">Parámetros inválidos (moneda no soportada, rango de fechas inválido, etc.).</response>
    [HttpGet("balance")]
    [ProducesResponseType(typeof(RespuestaApi<BalanceConsolidadoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObtenerBalanceConsolidado([FromQuery] ReporteBalanceConsultaDto consulta)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var balance = await _servicioReportes.ObtenerBalanceConsolidadoAsync(consulta);
        var respuesta = RespuestaApi<BalanceConsolidadoDto>.Exito(
            data: balance,
            mensaje: "Ejecutado exitosamente",
            codigo: 200
        );
        return Ok(respuesta);
    }
}
