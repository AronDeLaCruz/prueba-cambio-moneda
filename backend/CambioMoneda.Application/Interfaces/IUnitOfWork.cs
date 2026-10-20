namespace CambioMoneda.Application.Interfaces;

public interface IUnitOfWork
{
    Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
}