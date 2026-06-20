using System.ComponentModel.DataAnnotations;
using BancoSol.Domain.Enums;

namespace BancoSol.Application.DTOs;

public class CrearTransaccionDto
{
    
    [Required(ErrorMessage = "El monto es requerido")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
    public decimal Monto { get; set; }

    
    [Required(ErrorMessage = "La descripción es requerida")]
    [MaxLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    
    [Required(ErrorMessage = "La fecha es requerida")]
    public DateTime Fecha { get; set; }

    
    [Required(ErrorMessage = "El origen es requerido")]
    [MaxLength(100)]
    public string Origen { get; set; } = string.Empty;

    
    [Required(ErrorMessage = "La moneda es requerida")]
    public string Moneda { get; set; } = string.Empty;

    
    [Required(ErrorMessage = "El tipo de transacción es requerido")]
    [Range(1, 2, ErrorMessage = "El tipo debe ser 1 (Ingreso) o 2 (Egreso)")]
    public TipoTransaccion Tipo { get; set; }

    
    [Required(ErrorMessage = "El usuario creador es requerido")]
    [MaxLength(200)]
    public string CreadoPor { get; set; } = string.Empty;
}
