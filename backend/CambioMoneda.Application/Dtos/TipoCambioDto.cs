namespace CambioMoneda.Application.Dtos;

public record TipoCambioDto(int Id, string MonedaOrigen, string MonedaDestino, decimal Tasa, DateTime FechaActualizacion);