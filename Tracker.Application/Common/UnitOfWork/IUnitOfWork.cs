using Tracker.Application.Common.Repositories;

namespace Tracker.Application.Common.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
