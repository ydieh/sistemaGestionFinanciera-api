using BancoSol.Application.DTOs;

namespace BancoSol.Application.Interfaces;

public interface IServicioReportes
{
    
    Task<TipoCambioDto> ObtenerTipoCambioActualAsync();

    
    Task<BalanceConsolidadoDto> ObtenerBalanceConsolidadoAsync(ReporteBalanceConsultaDto consulta);
}
