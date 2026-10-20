namespace CambioMoneda.Application.Dtos;

public record TipoCambioRequest(string MonedaOrigen, string MonedaDestino, decimal Tasa);
