namespace BancoSol.Domain.Excepciones;

public sealed class RangoFechasInvalidoException : DomainException
{
    public RangoFechasInvalidoException(string mensaje) : base(mensaje) { }
}
