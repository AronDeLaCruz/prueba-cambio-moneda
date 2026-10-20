using CambioMoneda.Application.Interfaces;
using CambioMoneda.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CambioMoneda.Infrastructure.Persistence;

public class TipoCambioRepository(AppDbContext db) : ITipoCambioRepository
{
    public Task<List<TipoCambio>> GetAllAsync() => db.TiposCambio.AsNoTracking().ToListAsync();
    public Task<TipoCambio?> GetByIdAsync(int id) => db.TiposCambio.FirstOrDefaultAsync(x => x.Id == id);
    public Task<TipoCambio?> GetByParAsync(string o, string d) =>
        db.TiposCambio.FirstOrDefaultAsync(x => x.MonedaOrigen == o && x.MonedaDestino == d);
    public async Task AddAsync(TipoCambio e) => await db.TiposCambio.AddAsync(e);
    public void Remove(TipoCambio e) => db.TiposCambio.Remove(e);
}