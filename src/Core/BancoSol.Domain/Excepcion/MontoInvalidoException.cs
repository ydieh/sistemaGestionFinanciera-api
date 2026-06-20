namespace BancoSol.Domain.Excepciones;

public sealed class MontoInvalidoException : DomainException
{
    public MontoInvalidoException(string mensaje) : base(mensaje) { }
}
