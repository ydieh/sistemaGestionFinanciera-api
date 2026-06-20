namespace BancoSol.Domain.Excepciones;

public sealed class TipoTransaccionInvalidoException : DomainException
{
    public TipoTransaccionInvalidoException(string mensaje) : base(mensaje) { }
}
