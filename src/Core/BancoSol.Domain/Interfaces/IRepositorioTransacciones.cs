using BancoSol.Domain.Entidades;

namespace BancoSol.Domain.Interfaces;

public interface IRepositorioTransacciones
{
    Task<Transaccion> CrearAsync(Transaccion transaccion);
    Task<IEnumerable<Transaccion>> ObtenerTodasAsync(string? creadoPor = null);

    Task<Transaccion?> ObtenerPorIdAsync(int id);
}
