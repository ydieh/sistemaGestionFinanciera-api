namespace BancoSol.Application.DTOs;

public record RespuestaApi<T>(
    int Codigo,
    string Mensaje,
    T? Data
)
{
    
    public static RespuestaApi<T> Exito(T data, string mensaje = "Ejecutado exitosamente", int codigo = 201)
        => new(codigo, mensaje, data);

    public static RespuestaApi<object> Error(string mensaje, int codigo = 400)
        => new(codigo, mensaje, null);
}
