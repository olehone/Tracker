using Tracker.Application.Common.Repositories;
using Tracker.Domain.Results;

namespace Tracker.Application.Common.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository UserRepository { get; }
    IWorkspaceRepository WorkspaceRepository{ get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
    Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default);
}
