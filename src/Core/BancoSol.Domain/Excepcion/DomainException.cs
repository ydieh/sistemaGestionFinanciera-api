namespace BancoSol.Domain.Excepciones;

public abstract class DomainException : Exception
{
    protected DomainException(string mensaje) : base(mensaje) { }
}
