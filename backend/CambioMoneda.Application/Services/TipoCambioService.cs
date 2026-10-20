using CambioMoneda.Application.Dtos;
using CambioMoneda.Application.Interfaces;
using CambioMoneda.Domain.Entities;
using CambioMoneda.Domain.Exceptions;

namespace CambioMoneda.Application.Services;

public class TipoCambioService(ITipoCambioRepository repo, IUnitOfWork uow) : ITipoCambioService
{
    public async Task<List<TipoCambioDto>> GetAllAsync() =>
        (await repo.GetAllAsync()).Select(ToDto).ToList();

    public async Task<TipoCambioDto> GetByIdAsync(int id) =>
        ToDto(await repo.GetByIdAsync(id) ?? throw new NotFoundException($"Tipo de cambio {id} no existe."));

    public Task<TipoCambioDto> CreateAsync(TipoCambioRequest r) =>
        uow.ExecuteInTransactionAsync(async () =>
        {
            var (origen, destino) = Validar(r);
            if (await repo.GetByParAsync(origen, destino) is not null)
                throw new ConflictException($"Ya existe el tipo de cambio {origen}->{destino}.");

            var entity = new TipoCambio
            {
                MonedaOrigen = origen, MonedaDestino = destino,
                Tasa = r.Tasa, FechaActualizacion = DateTime.UtcNow
            };
            await repo.AddAsync(entity);
            return ToDto(entity);
        });

    public Task<TipoCambioDto> UpdateAsync(int id, TipoCambioRequest r) =>
        uow.ExecuteInTransactionAsync(async () =>
        {
            var (origen, destino) = Validar(r);
            var entity = await repo.GetByIdAsync(id) ?? throw new NotFoundException($"Tipo de cambio {id} no existe.");

            var duplicado = await repo.GetByParAsync(origen, destino);
            if (duplicado is not null && duplicado.Id != id)
                throw new ConflictException($"Ya existe el tipo de cambio {origen}->{destino}.");

            entity.MonedaOrigen = origen;
            entity.MonedaDestino = destino;
            entity.Tasa = r.Tasa;
            entity.FechaActualizacion = DateTime.UtcNow;
            return ToDto(entity);
        });

    public Task DeleteAsync(int id) =>
        uow.ExecuteInTransactionAsync(async () =>
        {
            var entity = await repo.GetByIdAsync(id) ?? throw new NotFoundException($"Tipo de cambio {id} no existe.");
            repo.Remove(entity);
            return true;
        });

    private static (string, string) Validar(TipoCambioRequest r)
    {
        var o = r.MonedaOrigen?.Trim().ToUpperInvariant() ?? "";
        var d = r.MonedaDestino?.Trim().ToUpperInvariant() ?? "";
        if (o.Length != 3 || d.Length != 3) throw new BadRequestException("Las monedas deben ser códigos de 3 letras (ej. USD).");
        if (o == d) throw new BadRequestException("La moneda origen y destino no pueden ser iguales.");
        if (r.Tasa <= 0) throw new BadRequestException("El tipo de cambio debe ser mayor a 0.");
        return (o, d);
    }

    private static TipoCambioDto ToDto(TipoCambio t) =>
        new(t.Id, t.MonedaOrigen, t.MonedaDestino, t.Tasa, t.FechaActualizacion);
}