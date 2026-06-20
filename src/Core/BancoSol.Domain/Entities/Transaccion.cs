using BancoSol.Domain.Comun;
using BancoSol.Domain.Enums;
using BancoSol.Domain.Excepciones;
using BancoSol.Domain.ValueObjects;

namespace BancoSol.Domain.Entidades;

public class Transaccion : EntidadBase
{
    public decimal Monto { get; private set; }
    public string Descripcion { get; private set; } = string.Empty;
    public DateTime Fecha { get; private set; }
    public string Origen { get; private set; } = string.Empty; 
    public Moneda Moneda { get; private set; } = null!;
    public TipoTransaccion Tipo { get; private set; }
    public string CreadoPor { get; private set; } = string.Empty;

    private Transaccion() { }

    private Transaccion(
        decimal monto,
        string descripcion,
        DateTime fecha,
        string origen,
        Moneda moneda,
        TipoTransaccion tipo,
        string creadoPor)
    {
        Monto = monto;
        Descripcion = descripcion;
        Fecha = fecha;
        Origen = origen;
        Moneda = moneda;
        Tipo = tipo;
        CreadoPor = creadoPor;
        FechaCreacion = DateTime.UtcNow;
    }

    
    public static Transaccion Crear(
        decimal monto,
        string descripcion,
        DateTime fecha,
        string origen,
        string codigoMoneda,
        TipoTransaccion tipo,
        string creadoPor)
    {
        ValidarMonto(monto);
        ValidarDescripcion(descripcion);
        ValidarOrigen(origen);
        ValidarCreadoPor(creadoPor);
        ValidarTipo(tipo);

        var moneda = Moneda.Crear(codigoMoneda); 

        return new Transaccion(monto, descripcion.Trim(), fecha, origen.Trim(), moneda, tipo, creadoPor.Trim());
    }

    private static void ValidarMonto(decimal monto)
    {
        if (monto <= 0)
            throw new MontoInvalidoException("El monto debe ser mayor a cero.");
    }

    private static void ValidarDescripcion(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new MontoInvalidoException("La descripción es requerida.");
    }

    private static void ValidarOrigen(string origen)
    {
        if (string.IsNullOrWhiteSpace(origen))
            throw new MontoInvalidoException("El origen es requerido.");
    }

    private static void ValidarCreadoPor(string creadoPor)
    {
        if (string.IsNullOrWhiteSpace(creadoPor))
            throw new MontoInvalidoException("El usuario creador es requerido.");
    }

    private static void ValidarTipo(TipoTransaccion tipo)
    {
        if (tipo != TipoTransaccion.Ingreso && tipo != TipoTransaccion.Egreso)
            throw new TipoTransaccionInvalidoException(
                $"Tipo de transacción '{(int)tipo}' no válido. Solo se acepta 1 (Ingreso) o 2 (Egreso).");
    }

    
    public decimal MontoConSigno() => Tipo == TipoTransaccion.Ingreso ? Monto : -Monto;
}
