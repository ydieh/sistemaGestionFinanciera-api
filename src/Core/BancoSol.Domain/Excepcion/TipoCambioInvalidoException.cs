namespace BancoSol.Domain.Excepciones;

public sealed class TipoCambioInvalidoException : DomainException
{
    public TipoCambioInvalidoException(string mensaje) : base(mensaje) { }
}
