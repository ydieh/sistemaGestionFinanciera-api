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
}
