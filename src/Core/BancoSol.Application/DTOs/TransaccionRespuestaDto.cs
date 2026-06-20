using BancoSol.Domain.Enums;

namespace BancoSol.Application.DTOs;


public class TransaccionRespuestaDto
{
    public int Id { get; set; }
    public decimal Monto { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public string Origen { get; set; } = string.Empty;
    public string Moneda { get; set; } = string.Empty;
    public TipoTransaccion Tipo { get; set; }
    public string CreadoPor { get; set; } = string.Empty;
    public DateTime FechaCreacion { get; set; }
}
