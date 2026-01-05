using Tracker.Application.Common.Repositories;
using Tracker.Domain.Results;

namespace Tracker.Application.Common.UnitOfWork;

public interface IUnitOfWork : IAsyncDisposable
{
    IUserRepository UserRepository { get; }
    IWorkspaceRepository WorkspaceRepository{ get; }
    IUserWorkspaceRepository UserWorkspaceRepository { get; }
    IBoardRepository BoardRepository{ get; }
    IUserBoardRepository UserBoardRepository { get; }
    IBoardListRepository BoardListRepository{ get; }
    IBoardItemRepository BoardItemRepository{ get; }
    IRefreshTokenRepository RefreshTokenRepository { get; }
    Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default);
}
