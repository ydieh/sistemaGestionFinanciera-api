using BancoSol.Domain.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BancoSol.Domain.Interfaces;

public interface IRepositorioTransacciones
{
    Task<Transaccion> CrearAsync(Transaccion transaccion, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaccion>> ObtenerTodasAsync(CancellationToken cancellationToken = default);
    Task<Transaccion?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Transaccion>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken cancellationToken = default);
}
