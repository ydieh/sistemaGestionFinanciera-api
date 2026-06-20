namespace BancoSol.Domain.Interfaces;

public interface IServicioTipoCambio
{
    Task<decimal> ObtenerTipoCambioUsdBobAsync();
}
