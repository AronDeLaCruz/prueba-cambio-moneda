using CambioMoneda.Application.Interfaces;

namespace CambioMoneda.Infrastructure.Persistence;

public class UnitOfWork(AppDbContext db) : IUnitOfWork
{
    public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
    {
        await using var tx = await db.Database.BeginTransactionAsync();
        try
        {
            var result = await action();
            await db.SaveChangesAsync();
            await tx.CommitAsync();
            return result;
        }
        catch
        {
            await tx.RollbackAsync();
            throw;
        }
    }
}