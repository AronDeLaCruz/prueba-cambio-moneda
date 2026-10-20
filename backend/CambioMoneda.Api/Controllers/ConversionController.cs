using CambioMoneda.Application.Dtos;
using CambioMoneda.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CambioMoneda.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/conversion")]
public class ConversionController(IConversionService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ConversionResponse>> Convertir(ConversionRequest r) => await service.ConvertirAsync(r);
}