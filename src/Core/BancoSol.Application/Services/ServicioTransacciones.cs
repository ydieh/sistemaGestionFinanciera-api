using BancoSol.Application.DTOs;
using BancoSol.Application.Interfaces;
using BancoSol.Domain.Entidades;
using BancoSol.Domain.Interfaces;

namespace BancoSol.Application.Servicios;


public class ServicioTransacciones : IServicioTransacciones
{
    private readonly IRepositorioTransacciones _repositorio;

    public ServicioTransacciones(IRepositorioTransacciones repositorio)
    {
        _repositorio = repositorio;
    }

    public async Task<TransaccionRespuestaDto> RegistrarTransaccionAsync(CrearTransaccionDto dto)
    {
        var transaccion = Transaccion.Crear(
            monto: dto.Monto,
            descripcion: dto.Descripcion,
            fecha: dto.Fecha,
            origen: dto.Origen,
            codigoMoneda: dto.Moneda,
            tipo: dto.Tipo,
            creadoPor: dto.CreadoPor);

        var transaccionCreada = await _repositorio.CrearAsync(transaccion);

        return TransaccionMapper.ADto(transaccionCreada);
    }
}
