using CambioMoneda.Application.Dtos;
using CambioMoneda.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CambioMoneda.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/tipos-cambio")]
public class TiposCambioController(ITipoCambioService service) : ControllerBase
{
    [HttpGet] public async Task<ActionResult<List<TipoCambioDto>>> GetAll() => await service.GetAllAsync();

    [HttpGet("{id:int}")] public async Task<ActionResult<TipoCambioDto>> Get(int id) => await service.GetByIdAsync(id);

    [HttpPost]
    public async Task<ActionResult<TipoCambioDto>> Create(TipoCambioRequest r)
    {
        var created = await service.CreateAsync(r);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")] public async Task<ActionResult<TipoCambioDto>> Update(int id, TipoCambioRequest r) => await service.UpdateAsync(id, r);

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) { await service.DeleteAsync(id); return NoContent(); }
}