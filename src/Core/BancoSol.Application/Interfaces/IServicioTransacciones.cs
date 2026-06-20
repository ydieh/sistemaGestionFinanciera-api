using BancoSol.Application.DTOs;

namespace BancoSol.Application.Interfaces;

public interface IServicioTransacciones
{
    Task<TransaccionRespuestaDto> RegistrarTransaccionAsync(CrearTransaccionDto dto);
    Task<IEnumerable<TransaccionRespuestaDto>> ObtenerTodasAsync(string? creadoPor = null);
}
