using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetForCurrentUser;

public sealed class GetWorkspacesForCurrentUserQueryHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetWorkspacesForCurrentUserQuery, Result<List<WorkspaceSummaryDto>>>
{
    public async Task<Result<List<WorkspaceSummaryDto>>> Handle(
        GetWorkspacesForCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated())
        {
            return AuthErrors.Unauthenticated;
        }
        var userId = userContext.GetUserId();

        await using var uow = unitOfWorkFactory.Create();
        var workspaces = await uow.WorkspaceRepository.GetByUserAsync(userId);

        return workspaces is null
            ? Error.NotFound("Workspaces", "user")
            : workspaces.Select(workspace => workspace.ToSummaryDto()).ToList();
    }
}
