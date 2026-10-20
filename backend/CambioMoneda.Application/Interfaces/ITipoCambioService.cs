using CambioMoneda.Application.Dtos;
 
namespace CambioMoneda.Application.Interfaces;

public interface ITipoCambioService
{
    Task<List<TipoCambioDto>> GetAllAsync();
    Task<TipoCambioDto> GetByIdAsync(int id);
    Task<TipoCambioDto> CreateAsync(TipoCambioRequest request);
    Task<TipoCambioDto> UpdateAsync(int id, TipoCambioRequest request);
    Task DeleteAsync(int id);
}
