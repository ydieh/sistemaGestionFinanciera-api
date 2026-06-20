using System.Text.Json;
using BancoSol.Domain.Interfaces;
using BancoSol.Infrastructure.ServiciosExternos.Modelos;
using Microsoft.Extensions.Logging;

namespace BancoSol.Infrastructure.ServiciosExternos;

public class ServicioTipoCambioHexaRate : IServicioTipoCambio
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ServicioTipoCambioHexaRate> _logger;
    private const string UrlApi = "https://hexarate.paikama.co/api/rates/USD/BOB/latest";

    public ServicioTipoCambioHexaRate(HttpClient httpClient, ILogger<ServicioTipoCambioHexaRate> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<decimal> ObtenerTipoCambioUsdBobAsync()
    {
        try
        {
            var respuesta = await _httpClient.GetAsync(UrlApi);
            respuesta.EnsureSuccessStatusCode();

            var json = await respuesta.Content.ReadAsStringAsync();
            var opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var resultado = JsonSerializer.Deserialize<RespuestaHexaRate>(json, opciones);

            if (resultado?.Data?.Mid is null or 0)
                throw new InvalidOperationException("HexaRate no devolvió un tipo de cambio válido.");

            _logger.LogInformation("Tipo de cambio obtenido: 1 USD = {Tasa} BOB", resultado.Data.Mid);
            return resultado.Data.Mid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar HexaRate API");
            throw new InvalidOperationException("No se pudo obtener el tipo de cambio. Intente más tarde.", ex);
        }
    }
}
