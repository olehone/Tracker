namespace Tracker.Application.Common.Repositories;
public interface IRepository<TEntity, TId> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TId id);
    Task<IReadOnlyList<TEntity>> GetAllAsync();
    Task AddAsync(TEntity entity);
    void Update(TEntity entity);
    Task RemoveAsync(TId id);
}