using Tracker.Application;
using Tracker.Domain.Entities;
using Tracker.Application.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tracker.Persistence.Repositories;
public class Repository<TEntity, TId> : IRepository<TEntity, TId> where TEntity : class
{
    protected DbSet<TEntity> _dbSet;
    protected ApplicationDbContext _dbContext;
    public Repository(ApplicationDbContext applicationDbContext)
    {
        _dbContext = applicationDbContext;
        _dbSet = applicationDbContext.Set<TEntity>();
    }
    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(TId id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task RemoveAsync(TId id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }
}
