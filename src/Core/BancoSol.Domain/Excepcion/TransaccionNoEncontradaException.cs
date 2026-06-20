namespace BancoSol.Domain.Excepciones;

public sealed class TransaccionNoEncontradaException : DomainException
{
    public TransaccionNoEncontradaException(int id)
        : base($"No se encontró ninguna transacción con el ID {id}.") { }
}
