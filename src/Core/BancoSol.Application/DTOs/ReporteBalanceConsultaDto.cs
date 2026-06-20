using System.ComponentModel.DataAnnotations;

namespace BancoSol.Application.DTOs;

public class ReporteBalanceConsultaDto : IValidatableObject
{
    [Required(ErrorMessage = "La fecha de inicio es requerida")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es requerida")]
    public DateTime FechaFin { get; set; }

    [Required(ErrorMessage = "La moneda es requerida")]
    public string Moneda { get; set; } = string.Empty;

   
    public string? CreadoPor { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FechaFin < FechaInicio)
        {
            yield return new ValidationResult(
                "La fecha de fin no puede ser anterior a la fecha de inicio.",
                new[] { nameof(FechaFin) }
            );
        }
    }

}
