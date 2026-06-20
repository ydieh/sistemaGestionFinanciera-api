using BancoSol.Domain.Entidades;
using BancoSol.Domain.Interfaces;
using BancoSol.Infrastructure.Datos;
using Microsoft.EntityFrameworkCore;

namespace BancoSol.Infrastructure.Repositorios;

public class RepositorioTransacciones : IRepositorioTransacciones
{
    private readonly BancoSolDbContext _contexto;

    public RepositorioTransacciones(BancoSolDbContext contexto)
    {
        _contexto = contexto;
    }

    public async Task<Transaccion> CrearAsync(Transaccion transaccion)
    {
        _contexto.Transacciones.Add(transaccion);
        await _contexto.SaveChangesAsync();
        return transaccion;
    }

    public async Task<IEnumerable<Transaccion>> ObtenerTodasAsync()
    {
        return await _contexto.Transacciones
            .OrderByDescending(t => t.Fecha)
            .ToListAsync();
    }

    public async Task<Transaccion?> ObtenerPorIdAsync(int id)
    {
        return await _contexto.Transacciones.FindAsync(id);
    }

    public async Task<IEnumerable<Transaccion>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        return await _contexto.Transacciones
            .Where(t => t.Fecha >= fechaInicio && t.Fecha <= fechaFin)
            .OrderByDescending(t => t.Fecha)
            .ToListAsync();
    }
}
