using BancoSol.Domain.Entidades;
using BancoSol.Domain.Enums;
using BancoSol.Domain.Excepciones;
using Xunit;

namespace BancoSol.Tests.Dominio;

public class TransaccionTests
{
    private static Transaccion CrearTransaccionValida(
        decimal monto = 1000,
        string moneda = "BOB",
        TipoTransaccion tipo = TipoTransaccion.Ingreso) =>
        Transaccion.Crear(monto, "Sueldo de junio", DateTime.Now, "sueldo", moneda, tipo, "juan@email.com");

    [Theory]
    [InlineData("EUR")]
    [InlineData("GBP")]
    [InlineData("ARS")]
    [InlineData("")]
    public void Crear_ConMonedaNoSoportada_LanzaMonedaInvalidaException(string monedaInvalida)
    {
        Assert.Throws<MonedaInvalidaException>(() => CrearTransaccionValida(moneda: monedaInvalida));
    }

    [Theory]
    [InlineData("BOB")]
    [InlineData("USD")]
    [InlineData("bob")] // debe aceptar minúsculas
    public void Crear_ConMonedaValida_CreaTransaccionCorrectamente(string monedaValida)
    {
        var transaccion = CrearTransaccionValida(moneda: monedaValida);

        Assert.Equal(monedaValida.ToUpperInvariant(), transaccion.Moneda.Codigo);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Crear_ConMontoMenorOIgualACero_LanzaMontoInvalidoException(decimal montoInvalido)
    {
        Assert.Throws<MontoInvalidoException>(() => CrearTransaccionValida(monto: montoInvalido));
    }

    [Fact]
    public void MontoConSigno_ParaIngreso_RetornaMontoPositivo()
    {
        var transaccion = CrearTransaccionValida(monto: 500, tipo: TipoTransaccion.Ingreso);

        Assert.Equal(500, transaccion.MontoConSigno());
    }

    [Fact]
    public void MontoConSigno_ParaEgreso_RetornaMontoNegativo()
    {
        var transaccion = CrearTransaccionValida(monto: 500, tipo: TipoTransaccion.Egreso);

        Assert.Equal(-500, transaccion.MontoConSigno());
    }

    [Fact]
    public void Crear_ConTipoFueraDeRango_LanzaTipoTransaccionInvalidoException()
    {
        var tipoInvalido = (TipoTransaccion)99;

        Assert.Throws<TipoTransaccionInvalidoException>(
            () => CrearTransaccionValida(tipo: tipoInvalido));
    }
}
