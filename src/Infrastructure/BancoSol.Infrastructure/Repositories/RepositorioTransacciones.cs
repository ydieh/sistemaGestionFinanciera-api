using BancoSol.Domain.Entidades;
using BancoSol.Domain.Interfaces;
using BancoSol.Infrastructure.Datos;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BancoSol.Infrastructure.Repositorios;

public class RepositorioTransacciones : IRepositorioTransacciones
{
    private readonly BancoSolDbContext _contexto;

    public RepositorioTransacciones(BancoSolDbContext contexto) => _contexto = contexto;

    public async Task<Transaccion> CrearAsync(Transaccion transaccion, CancellationToken cancellationToken = default)
    {
        var resultado = await _contexto.Transacciones.AddAsync(transaccion);
        await _contexto.SaveChangesAsync();
        return resultado.Entity;
    }

    public async Task<IEnumerable<Transaccion>> ObtenerTodasAsync(CancellationToken cancellationToken = default)
    {
        var query = _contexto.Transacciones.AsQueryable();
        return await query
            .OrderByDescending(t => t.Fecha)
            .ToListAsync(cancellationToken);
    }

    public async Task<Transaccion?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _contexto.Transacciones.FindAsync(id);
    }
 
    public async Task<IEnumerable<Transaccion>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin, CancellationToken cancellationToken = default)
    {
        var query = _contexto.Transacciones.AsQueryable();
        query = query.Where(t => t.Fecha >= fechaInicio && t.Fecha <= fechaFin);
        return await query
            .OrderByDescending(t => t.Fecha)
            .ToListAsync();
    }

}
