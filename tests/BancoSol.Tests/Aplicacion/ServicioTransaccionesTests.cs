using BancoSol.Application.DTOs;
using BancoSol.Application.Servicios;
using BancoSol.Domain.Entidades;
using BancoSol.Domain.Enums;
using BancoSol.Domain.Excepciones;
using BancoSol.Domain.Interfaces;
using Moq;
using Xunit;

namespace BancoSol.Tests.Aplicacion;

public class ServicioTransaccionesTests
{
    private readonly Mock<IRepositorioTransacciones> _repositorioMock;
    private readonly ServicioTransacciones _servicio;

    public ServicioTransaccionesTests()
    {
        _repositorioMock = new Mock<IRepositorioTransacciones>();
        _servicio = new ServicioTransacciones(_repositorioMock.Object);
    }

    private static CrearTransaccionDto DtoValido(string moneda = "BOB", TipoTransaccion tipo = TipoTransaccion.Ingreso) => new()
    {
        Monto = 5000,
        Descripcion = "Sueldo de Junio",
        Fecha = new DateTime(2025, 12, 1),
        Origen = "sueldo",
        Moneda = moneda,
        Tipo = tipo,
        CreadoPor = "pepito.perez@email.com"
    };

    [Fact]
    public async Task RegistrarTransaccionAsync_ConDatosValidos_PersisteYRetornaDto()
    {
        var dto = DtoValido();

        _repositorioMock
            .Setup(r => r.CrearAsync(It.IsAny<Transaccion>()))
            .ReturnsAsync((Transaccion t) => t);

        var resultado = await _servicio.RegistrarTransaccionAsync(dto);

        Assert.Equal(dto.Monto, resultado.Monto);
        Assert.Equal("BOB", resultado.Moneda);
        Assert.Equal(TipoTransaccion.Ingreso, resultado.Tipo);
        _repositorioMock.Verify(r => r.CrearAsync(It.IsAny<Transaccion>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarTransaccionAsync_ConMonedaNoSoportada_LanzaMonedaInvalidaException_YNoPersiste()
    {
        var dto = DtoValido(moneda: "EUR");

        await Assert.ThrowsAsync<MonedaInvalidaException>(() => _servicio.RegistrarTransaccionAsync(dto));

        _repositorioMock.Verify(r => r.CrearAsync(It.IsAny<Transaccion>()), Times.Never);
    }

    [Fact]
    public async Task RegistrarTransaccionAsync_ConTipoEgreso_RegistraCorrectamente()
    {
        var dto = DtoValido(tipo: TipoTransaccion.Egreso);

        _repositorioMock
            .Setup(r => r.CrearAsync(It.IsAny<Transaccion>()))
            .ReturnsAsync((Transaccion t) => t);

        var resultado = await _servicio.RegistrarTransaccionAsync(dto);

        Assert.Equal(TipoTransaccion.Egreso, resultado.Tipo);
    }
    [Fact]
    public async Task ObtenerTodasAsync_ConTransaccionesExistentes_RetornaListadoMapeadoCorrectamente()
    {
        var transacciones = new List<Transaccion>
        {
            Transaccion.Crear(5000, "Sueldo de junio", new DateTime(2025, 12, 1), "sueldo", "BOB", TipoTransaccion.Ingreso, "pepito.perez@email.com"),
            Transaccion.Crear(100, "Freelance", new DateTime(2025, 12, 5), "freelance", "USD", TipoTransaccion.Ingreso, "maria.lopez@email.com")
        };

        _repositorioMock
            .Setup(r => r.ObtenerTodasAsync(null))
            .ReturnsAsync(transacciones);

        var resultado = (await _servicio.ObtenerTodasAsync()).ToList();

        Assert.Equal(2, resultado.Count);
        Assert.Equal("BOB", resultado[0].Moneda);
        Assert.Equal("USD", resultado[1].Moneda);
        _repositorioMock.Verify(r => r.ObtenerTodasAsync(null), Times.Once);
    }

    [Fact]
    public async Task ObtenerTodasAsync_SinTransaccionesRegistradas_RetornaListaVacia()
    {
        _repositorioMock
            .Setup(r => r.ObtenerTodasAsync(null))
            .ReturnsAsync(Enumerable.Empty<Transaccion>());

        var resultado = await _servicio.ObtenerTodasAsync();

        Assert.Empty(resultado);
    }

    [Fact]
    public async Task ObtenerTodasAsync_ConFiltroCreadoPor_DelegaElFiltroAlRepositorio()
    {
        const string creadoPor = "pepito.perez@email.com";
        var transaccionesDeJuan = new List<Transaccion>
        {
            Transaccion.Crear(5000, "Sueldo de junio", new DateTime(2025, 12, 1), "sueldo", "BOB", TipoTransaccion.Ingreso, creadoPor)
        };

        _repositorioMock
            .Setup(r => r.ObtenerTodasAsync(creadoPor))
            .ReturnsAsync(transaccionesDeJuan);

        var resultado = (await _servicio.ObtenerTodasAsync(creadoPor)).ToList();

        Assert.Single(resultado);
        _repositorioMock.Verify(r => r.ObtenerTodasAsync(creadoPor), Times.Once);
    }

}
