using CambioMoneda.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CambioMoneda.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TipoCambio> TiposCambio => Set<TipoCambio>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<TipoCambio>(e =>
        {
            e.Property(x => x.MonedaOrigen).HasMaxLength(3).IsRequired();
            e.Property(x => x.MonedaDestino).HasMaxLength(3).IsRequired();
            e.Property(x => x.Tasa).HasPrecision(18, 6);
            e.HasIndex(x => new { x.MonedaOrigen, x.MonedaDestino }).IsUnique();
            e.HasData(
                new TipoCambio { Id = 1, MonedaOrigen = "USD", MonedaDestino = "PEN", Tasa = 3.75m, FechaActualizacion = new DateTime(2026, 10, 19) },
                new TipoCambio { Id = 2, MonedaOrigen = "EUR", MonedaDestino = "PEN", Tasa = 4.10m, FechaActualizacion = new DateTime(2026, 10, 19) });
        });
    }
}