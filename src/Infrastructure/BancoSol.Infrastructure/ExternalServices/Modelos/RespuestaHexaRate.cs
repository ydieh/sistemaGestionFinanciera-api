namespace BancoSol.Infrastructure.ServiciosExternos.Modelos;

public class RespuestaHexaRate
{
    public DatosHexaRate? Data { get; set; }
}

public class DatosHexaRate
{
    public string? Base { get; set; }
    public string? Target { get; set; }
    public decimal Mid { get; set; } 
}
