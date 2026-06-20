using BancoSol.Domain.Entidades;
using BancoSol.Domain.Excepciones;
using BancoSol.Domain.ValueObjects;

namespace BancoSol.Domain.Servicios;


public static class CalculadoraBalance
{
    public static decimal Calcular(IEnumerable<Transaccion> ingresos, string codigoMonedaDestino, decimal tipoCambioUsdBob)
    {
        if (tipoCambioUsdBob <= 0)
            throw new TipoCambioInvalidoException("El tipo de cambio debe ser mayor a cero.");

        var monedaDestino = Moneda.Crear(codigoMonedaDestino);

        decimal total = 0;

        foreach (var ingreso in ingresos)
        {
            total += ConvertirAMonedaDestino(ingreso.Monto, ingreso.Moneda, monedaDestino, tipoCambioUsdBob);
        }

        return Math.Round(total, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal ConvertirAMonedaDestino(
        decimal monto, Moneda monedaOrigen, Moneda monedaDestino, decimal tipoCambioUsdBob)
    {
        if (monedaOrigen == monedaDestino)
            return monto;

        return monedaDestino.Codigo == Moneda.CodigoBOB
            ? monto * tipoCambioUsdBob
            : monto / tipoCambioUsdBob;
    }
}
