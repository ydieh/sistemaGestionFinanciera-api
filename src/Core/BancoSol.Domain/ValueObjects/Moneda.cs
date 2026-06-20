using BancoSol.Domain.Excepciones;

namespace BancoSol.Domain.ValueObjects;

public sealed class Moneda : IEquatable<Moneda>
{
    public const string CodigoBOB = "BOB";
    public const string CodigoUSD = "USD";

    private static readonly HashSet<string> CodigosValidos = new() { CodigoBOB, CodigoUSD };

    public string Codigo { get; }

    private Moneda(string codigo)
    {
        Codigo = codigo;
    }

    public static Moneda Crear(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new MonedaInvalidaException("El código de moneda no puede estar vacío.");

        var codigoNormalizado = codigo.Trim().ToUpperInvariant();

        if (!CodigosValidos.Contains(codigoNormalizado))
            throw new MonedaInvalidaException(
                $"Moneda '{codigo}' no soportada. Solo se aceptan: {string.Join(", ", CodigosValidos)}.");

        return new Moneda(codigoNormalizado);
    }

    public static Moneda BOB => new(CodigoBOB);
    public static Moneda USD => new(CodigoUSD);

    public static bool EsValida(string? codigo) =>
        !string.IsNullOrWhiteSpace(codigo) && CodigosValidos.Contains(codigo.Trim().ToUpperInvariant());

    public bool Equals(Moneda? other) => other is not null && Codigo == other.Codigo;
    public override bool Equals(object? obj) => Equals(obj as Moneda);
    public override int GetHashCode() => Codigo.GetHashCode();
    public override string ToString() => Codigo;

    public static bool operator ==(Moneda? a, Moneda? b) => a?.Equals(b) ?? b is null;
    public static bool operator !=(Moneda? a, Moneda? b) => !(a == b);
}
