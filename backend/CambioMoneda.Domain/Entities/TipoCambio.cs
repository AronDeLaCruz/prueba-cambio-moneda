namespace CambioMoneda.Domain.Entities;

public class TipoCambio{
    
    public int Id { get; set; }
    public string MonedaOrigen { get; set; } = default!;   // "USD"
    public string MonedaDestino { get; set; } = default!;  // "PEN"
    public decimal Tasa { get; set; }
    public DateTime FechaActualizacion { get; set; }
}