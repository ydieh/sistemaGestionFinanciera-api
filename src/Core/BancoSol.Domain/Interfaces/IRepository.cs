namespace BancoSol.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> CreateAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
