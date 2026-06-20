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
    [Fact]
    public async Task ObtenerPorIdAsync_ConIdExistente_RetornaDtoMapeado()
    {
        var transaccion = Transaccion.Crear(5000, "Sueldo de diciembre", new DateTime(2025, 12, 1), "sueldo", "BOB", TipoTransaccion.Ingreso, "juan.perez@email.com");

        _repositorioMock
            .Setup(r => r.ObtenerPorIdAsync(42))
            .ReturnsAsync(transaccion);

        var resultado = await _servicio.ObtenerPorIdAsync(42);

        Assert.Equal(5000, resultado.Monto);
        Assert.Equal("BOB", resultado.Moneda);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConIdInexistente_LanzaTransaccionNoEncontradaException()
    {
        _repositorioMock
            .Setup(r => r.ObtenerPorIdAsync(999))
            .ReturnsAsync((Transaccion?)null);

        await Assert.ThrowsAsync<TransaccionNoEncontradaException>(() => _servicio.ObtenerPorIdAsync(999));
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConCreadoPorDeOtroUsuario_LanzaTransaccionNoEncontradaException()
    {
        var transaccionDeJuan = Transaccion.Crear(5000, "Sueldo de diciembre", new DateTime(2025, 12, 1), "sueldo", "BOB", TipoTransaccion.Ingreso, "juan.perez@email.com");

        _repositorioMock
            .Setup(r => r.ObtenerPorIdAsync(42))
            .ReturnsAsync(transaccionDeJuan);

        await Assert.ThrowsAsync<TransaccionNoEncontradaException>(
            () => _servicio.ObtenerPorIdAsync(42, creadoPor: "maria.lopez@email.com"));
    }

    [Fact]
    public async Task ObtenerPorIdAsync_ConCreadoPorDelMismoUsuario_RetornaLaTransaccion()
    {
        const string creadoPor = "juan.perez@email.com";
        var transaccionDeJuan = Transaccion.Crear(5000, "Sueldo de diciembre", new DateTime(2025, 12, 1), "sueldo", "BOB", TipoTransaccion.Ingreso, creadoPor);

        _repositorioMock
            .Setup(r => r.ObtenerPorIdAsync(42))
            .ReturnsAsync(transaccionDeJuan);

        var resultado = await _servicio.ObtenerPorIdAsync(42, creadoPor);

        Assert.Equal(creadoPor, resultado.CreadoPor);
    }

}
