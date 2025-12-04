
using Tracker.Persistence.Repositories;
using Tracker.Application.Common.Repositories;
using Tracker.Application.Common.UnitOfWork;

namespace Tracker.Persistence;
internal class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;
    public IUserRepository Users { get; }

    public UnitOfWork (ApplicationDbContext applicationDbContext)
    {
        _db = applicationDbContext;
        Users = new UserRepository(_db);
    }
    public ValueTask DisposeAsync()
    {
        return _db.DisposeAsync();
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _db.SaveChangesAsync(cancellationToken);
    }
}
