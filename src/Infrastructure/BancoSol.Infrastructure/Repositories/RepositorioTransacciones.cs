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

    public async Task<Transaccion> CrearAsync(Transaccion transaccion)
    {
        var resultado = await _contexto.Transacciones.AddAsync(transaccion);
        await _contexto.SaveChangesAsync();
        return resultado.Entity;
    }

    public async Task<IEnumerable<Transaccion>> ObtenerTodasAsync(string? creadoPor = null)
    {
        var query = _contexto.Transacciones.AsQueryable();

        if (!string.IsNullOrWhiteSpace(creadoPor))
            query = query.Where(t => t.CreadoPor == creadoPor);

        return await query
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
