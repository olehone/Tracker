using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetAllForUser;

public sealed class GetAllWorkspacesByUserQueryHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetAllWorkspacesByUserQuery, Result<Paginated<WorkspaceSummaryDto>>>
{
    public async Task<Result<Paginated<WorkspaceSummaryDto>>> Handle(
        GetAllWorkspacesByUserQuery request,
        CancellationToken cancellationToken)
    {
        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }

        await using var uow = unitOfWorkFactory.Create();
        var userId = userContext.GetUserId();
        var user = await uow.UserRepository.GetByIdAsync(userId);
        if (user is null)
        {
            return AuthErrors.Unauthenticated;
        }

        var userRole = user.Role;
        if (request.Id != userId && userRole < GlobalRole.Admin)
        {
            return AuthErrors.Forbidden("You must to be admin to perform this action");
        }

        var skip = (request.Page - 1) * request.AmountInPage;

        var count = await uow.WorkspaceRepository
            .CountAllAsync(request.SearchQuery, request.Id);
        if (count == 0)
        {
            return Paginated<WorkspaceSummaryDto>.Empty();
        }

        var workspaces = await uow.WorkspaceRepository
            .GetAllAsync(skip, request.AmountInPage, request.SearchQuery, request.Id);

        var workspaceDtos = workspaces.Select(w => w.ToSummaryDto()).ToList();
        return new Paginated<WorkspaceSummaryDto>
        {
            Items = workspaceDtos,
            TotalCount = count
        };
    }
}
