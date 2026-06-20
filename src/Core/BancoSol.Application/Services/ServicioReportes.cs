using BancoSol.Application.DTOs;
using BancoSol.Application.Interfaces;
using BancoSol.Domain.Enums;
using BancoSol.Domain.Excepciones;
using BancoSol.Domain.Interfaces;
using BancoSol.Domain.Servicios;
using BancoSol.Domain.ValueObjects;

namespace BancoSol.Application.Servicios;

public class ServicioReportes : IServicioReportes
{
    private readonly IRepositorioTransacciones _repositorio;
    private readonly IServicioTipoCambio _servicioTipoCambio;

    public ServicioReportes(IRepositorioTransacciones repositorio, IServicioTipoCambio servicioTipoCambio)
    {
        _repositorio = repositorio;
        _servicioTipoCambio = servicioTipoCambio;
    }

    public async Task<TipoCambioDto> ObtenerTipoCambioActualAsync()
    {
        var tasa = await _servicioTipoCambio.ObtenerTipoCambioUsdBobAsync();

        return new TipoCambioDto
        {
            Origen = Moneda.CodigoUSD,
            Destino = Moneda.CodigoBOB,
            Tasa = tasa,
            ConsultadoEn = DateTime.UtcNow
        };
    }

    public async Task<BalanceConsolidadoDto> ObtenerBalanceConsolidadoAsync(ReporteBalanceConsultaDto consulta)
    {
        ValidarRangoFechas(consulta.FechaInicio, consulta.FechaFin);

        var fechaFinInclusiva = consulta.FechaFin.Date.AddDays(1).AddTicks(-1);

        var transaccionesEnRango = await _repositorio.ObtenerPorRangoFechasAsync(consulta.FechaInicio, fechaFinInclusiva);

        var ingresos = transaccionesEnRango
            .Where(t => t.Tipo == TipoTransaccion.Ingreso)
            .Where(t => string.IsNullOrWhiteSpace(consulta.CreadoPor) || t.CreadoPor == consulta.CreadoPor)
            .ToList();


        var tipoCambio = await _servicioTipoCambio.ObtenerTipoCambioUsdBobAsync();

        var monedaDestino = Moneda.Crear(consulta.Moneda);

        var balanceTotal = CalculadoraBalance.Calcular(ingresos, monedaDestino.Codigo, tipoCambio);

        return new BalanceConsolidadoDto
        {
            FechaInicio = consulta.FechaInicio,
            FechaFin = consulta.FechaFin,
            Moneda = monedaDestino.Codigo,
            BalanceTotal = balanceTotal,
            CantidadIngresosConsiderados = ingresos.Count,
            TipoCambioUsado = tipoCambio
        };
    }

    private static void ValidarRangoFechas(DateTime fechaInicio, DateTime fechaFin)
    {
        if (fechaFin < fechaInicio)
            throw new RangoFechasInvalidoException("La fecha de fin no puede ser anterior a la fecha de inicio.");
    }
}
