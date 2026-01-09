using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetAll;

public sealed class GetWorkspacesQueryHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetWorkspacesQuery, Result<Paginated<WorkspaceSummaryDto>>>
{
    public async Task<Result<Paginated<WorkspaceSummaryDto>>> Handle(
        GetWorkspacesQuery request,
        CancellationToken cancellationToken)
    {
        if (!userContext.IsAuthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        var userRole = userContext.GetUserRole();
        if (userRole < GlobalRole.Admin)
        {
            return AuthErrors.Forbidden();
        }

        await using var uow = unitOfWorkFactory.Create();

        int skip = (request.Page - 1) * request.AmountInPage;

        var count = await uow.WorkspaceRepository
            .CountAsync(request.SearchQuery);
        if (count == 0)
        {
            return Paginated<WorkspaceSummaryDto>.Empty();
        }
        
        var workspaces = await uow.WorkspaceRepository
            .GetAsync(skip, request.AmountInPage, request.SearchQuery);

        var workspaceDtos = workspaces.Select(w => w.ToSummaryDto()).ToList();
        return new Paginated<WorkspaceSummaryDto>
        {
            Items = workspaceDtos,
            TotalCount = count
        };
    }
}
