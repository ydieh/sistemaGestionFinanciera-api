using BancoSol.Domain.Entidades;

namespace BancoSol.Domain.Interfaces;

public interface IRepositorioTransacciones
{
    Task<Transaccion> CrearAsync(Transaccion transaccion);
    Task<IEnumerable<Transaccion>> ObtenerTodasAsync();
    Task<Transaccion?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<Transaccion>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin);
}
