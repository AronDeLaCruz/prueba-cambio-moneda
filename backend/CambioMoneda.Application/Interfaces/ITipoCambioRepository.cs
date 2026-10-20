using CambioMoneda.Application.Dtos;
using CambioMoneda.Domain.Entities;

namespace CambioMoneda.Application.Interfaces;

public interface ITipoCambioRepository
{
    Task<List<TipoCambio>> GetAllAsync();
    Task<TipoCambio?> GetByIdAsync(int id);
    Task<TipoCambio?> GetByParAsync(string origen, string destino);
    Task AddAsync(TipoCambio entity);
    void Remove(TipoCambio entity);
}
