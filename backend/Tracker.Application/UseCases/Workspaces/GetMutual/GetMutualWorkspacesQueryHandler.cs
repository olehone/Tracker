using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetMutual;

public sealed class GetMutualWorkspacesQueryHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetMutualWorkspacesQuery, Result<Paginated<WorkspaceSummaryDto>>>
{
    public async Task<Result<Paginated<WorkspaceSummaryDto>>> Handle(
        GetMutualWorkspacesQuery request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        var userId = userContext.GetUserId();

        await using var uow = unitOfWorkFactory.Create();

        int skip = (request.Page - 1) * request.AmountInPage;

        var count = await uow.WorkspaceRepository
            .CountMutualAsync(request.TargetUserId, userId, request.SearchQuery);

        if (count == 0)
        {
            return Paginated<WorkspaceSummaryDto>.Empty();
        }

        var workspaces = await uow.WorkspaceRepository
            .GetMutualAsync(request.TargetUserId, userId, skip, request.AmountInPage, request.SearchQuery);
        var workspaceDtos = workspaces.Select(w => w.ToSummaryDto()).ToList();

        return new Paginated<WorkspaceSummaryDto>
        {
            Items = workspaceDtos,
            TotalCount = count,
        };
    }
}
