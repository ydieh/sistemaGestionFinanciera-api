namespace BancoSol.Application.DTOs;

public class BalanceConsolidadoDto
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public decimal BalanceTotal { get; set; }
    public int CantidadIngresosConsiderados { get; set; }
    public decimal TipoCambioUsado { get; set; }
}
