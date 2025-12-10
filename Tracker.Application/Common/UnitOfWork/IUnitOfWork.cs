using Tracker.Application.Common.Repositories;
using Tracker.Application.Results;

namespace Tracker.Application.Common.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository UserRepository { get; }

    Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default);
}
