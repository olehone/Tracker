using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.GetForCurrentUser;

public sealed class GetBoardsForCurrentUserQueryHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetBoardsForCurrentUserQuery, Result<List<BoardSummaryDto>>>
{
    public async Task<Result<List<BoardSummaryDto>>> Handle(
        GetBoardsForCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }
        var userId = userContext.GetUserId();

        await using var uow = unitOfWorkFactory.Create();
        var workspaces = await uow.BoardRepository.GetByUserAsync(userId);

        return workspaces is null
            ? Error.NotFound("Board", "user")
            : workspaces.Select(workspace => workspace.ToSummaryDto()).ToList();
    }
}
