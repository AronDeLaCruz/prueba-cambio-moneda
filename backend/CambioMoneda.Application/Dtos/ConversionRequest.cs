namespace CambioMoneda.Application.Dtos;

public record ConversionRequest(decimal Monto, string MonedaOrigen, string MonedaDestino);