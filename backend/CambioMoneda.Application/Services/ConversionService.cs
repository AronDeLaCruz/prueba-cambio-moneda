using CambioMoneda.Application.Dtos;
using CambioMoneda.Application.Interfaces;
using CambioMoneda.Domain.Exceptions;

namespace CambioMoneda.Application.Services;

public class ConversionService(ITipoCambioRepository repo) : IConversionService
{
    public async Task<ConversionResponse> ConvertirAsync(ConversionRequest r)
    {
        var o = r.MonedaOrigen?.Trim().ToUpperInvariant() ?? "";
        var d = r.MonedaDestino?.Trim().ToUpperInvariant() ?? "";

        if (r.Monto <= 0) throw new BadRequestException("El monto debe ser mayor a 0.");
        if (o.Length != 3 || d.Length != 3) throw new BadRequestException("Monedas inválidas.");
        if (o == d) throw new BadRequestException("La moneda origen y destino no pueden ser iguales.");

        decimal tasa;
        var directo = await repo.GetByParAsync(o, d);
        if (directo is not null) tasa = directo.Tasa;
        else
        {
            // Decisión: si no existe el par directo, uso el inverso (1 / tasa)
            var inverso = await repo.GetByParAsync(d, o)
                ?? throw new NotFoundException($"No hay tipo de cambio registrado para {o}->{d}.");
            tasa = Math.Round(1m / inverso.Tasa, 6);
        }

        return new ConversionResponse(r.Monto, Math.Round(r.Monto * tasa, 2), o, d, tasa);
    }
}