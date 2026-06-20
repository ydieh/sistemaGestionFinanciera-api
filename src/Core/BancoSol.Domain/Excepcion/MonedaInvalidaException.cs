namespace BancoSol.Domain.Excepciones;

public sealed class MonedaInvalidaException : DomainException
{
    public MonedaInvalidaException(string mensaje) : base(mensaje) { }
}
