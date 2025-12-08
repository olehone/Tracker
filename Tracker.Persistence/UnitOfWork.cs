using Tracker.Persistence.Repositories;
using Tracker.Application.Common.Repositories;
using Tracker.Application.Common.UnitOfWork;

namespace Tracker.Persistence;

internal class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    private IUserRepository _userRepository = null!;
    public IUserRepository UserRepository
        => _userRepository ??= new UserRepository(_dbContext);

    public UnitOfWork(ApplicationDbContext applicationDbContext)
    {
        _dbContext = applicationDbContext;
    }

    public ValueTask DisposeAsync()
    {
        return _dbContext.DisposeAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
