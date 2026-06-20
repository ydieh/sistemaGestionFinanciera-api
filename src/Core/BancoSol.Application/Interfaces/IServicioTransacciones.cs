using BancoSol.Application.DTOs;

namespace BancoSol.Application.Interfaces;

public interface IServicioTransacciones
{
    Task<TransaccionRespuestaDto> RegistrarTransaccionAsync(CrearTransaccionDto dto, CancellationToken cancellationToken = default);
  
    Task<IEnumerable<TransaccionRespuestaDto>> ObtenerTodasAsync(string? creadoPor = null);
    Task<TransaccionRespuestaDto> ObtenerPorIdAsync(int id, string? creadoPor = null);
}
