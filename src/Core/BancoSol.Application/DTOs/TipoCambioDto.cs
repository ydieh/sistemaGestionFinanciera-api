namespace BancoSol.Application.DTOs;

public class TipoCambioDto
{
    public string Origen { get; set; } = "USD";
    public string Destino { get; set; } = "BOB";
    public decimal Tasa { get; set; }
    public DateTime ConsultadoEn { get; set; }
}
