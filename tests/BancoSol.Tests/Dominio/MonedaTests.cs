using BancoSol.Domain.Excepciones;
using BancoSol.Domain.ValueObjects;
using Xunit;

namespace BancoSol.Tests.Dominio;

public class MonedaTests
{
    [Fact]
    public void Crear_ConCodigoValido_RetornaInstanciaCorrecta()
    {
        var moneda = Moneda.Crear("BOB");

        Assert.Equal("BOB", moneda.Codigo);
    }

    [Fact]
    public void Crear_ConCodigoInvalido_LanzaMonedaInvalidaException()
    {
        Assert.Throws<MonedaInvalidaException>(() => Moneda.Crear("EUR"));
    }

    [Fact]
    public void DosMonedasConElMismoCodigo_SonIguales()
    {
        var moneda1 = Moneda.Crear("USD");
        var moneda2 = Moneda.Crear("usd"); 

        Assert.Equal(moneda1, moneda2);
        Assert.True(moneda1 == moneda2);
    }

    [Fact]
    public void EsValida_ConCodigoSoportado_RetornaTrue()
    {
        Assert.True(Moneda.EsValida("BOB"));
        Assert.True(Moneda.EsValida("USD"));
    }

    [Fact]
    public void EsValida_ConCodigoNoSoportado_RetornaFalse()
    {
        Assert.False(Moneda.EsValida("EUR"));
        Assert.False(Moneda.EsValida(null));
    }
}
