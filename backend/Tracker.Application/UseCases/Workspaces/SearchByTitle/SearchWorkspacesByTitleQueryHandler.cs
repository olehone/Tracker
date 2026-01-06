using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.SearchByTitle;

public sealed class SearchWorkspacesByTitleQueryHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<SearchWorkspacesByTitleQuery, Result<List<WorkspaceDto>>>
{
    public async Task<Result<List<WorkspaceDto>>> Handle(
        SearchWorkspacesByTitleQuery request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated())
        {
            return Result.FailureOf<List<WorkspaceDto>>(AuthErrors.Unauthenticated);
        }
        var userId = userContext.GetUserId();

        await using var uow = unitOfWorkFactory.Create();

        int skip = (request.Page - 1) * request.AmountInPage;
        var workspaces = await uow.WorkspaceRepository
            .SearchByTitleWithUserIdAsync(userId, request.Title, skip, request.AmountInPage);

        return workspaces is null
            ? Error.NotFound("Workspaces", "title")
            : workspaces.Select(workspace => workspace.ToDto()).ToList();
    }
}
