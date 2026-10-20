using CambioMoneda.Application.Dtos;
using CambioMoneda.Domain.Entities;

namespace CambioMoneda.Application.Interfaces;

 
 


public interface IConversionService
{
    Task<ConversionResponse> ConvertirAsync(ConversionRequest request);
}
