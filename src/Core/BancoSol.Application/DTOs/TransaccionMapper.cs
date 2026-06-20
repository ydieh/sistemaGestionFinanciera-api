using BancoSol.Application.DTOs;
using BancoSol.Domain.Entidades;

namespace BancoSol.Application.DTOs;

public static class TransaccionMapper
{
    public static TransaccionRespuestaDto ADto(Transaccion transaccion) => new()
    {
        Id = transaccion.Id,
        Monto = transaccion.Monto,
        Descripcion = transaccion.Descripcion,
        Fecha = transaccion.Fecha,
        Origen = transaccion.Origen,
        Moneda = transaccion.Moneda.Codigo,
        Tipo = transaccion.Tipo,
        CreadoPor = transaccion.CreadoPor,
        FechaCreacion = transaccion.FechaCreacion
    };

    

    public static IEnumerable<TransaccionRespuestaDto> ADtoLista(IEnumerable<Transaccion> transacciones) =>
        transacciones.Select(ADto);
}
