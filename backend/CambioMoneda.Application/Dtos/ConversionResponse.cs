namespace CambioMoneda.Application.Dtos;

public record ConversionResponse(decimal Monto, decimal MontoConTipoCambio, string MonedaOrigen, string MonedaDestino, decimal TipoCambio);
