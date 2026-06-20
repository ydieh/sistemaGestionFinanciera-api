namespace BancoSol.Domain.Comun;

public abstract class EntidadBase
{
    public int Id { get; protected set; }
    public DateTime FechaCreacion { get; protected set; } = DateTime.UtcNow;
}
