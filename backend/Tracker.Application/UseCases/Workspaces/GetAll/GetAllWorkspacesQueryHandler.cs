using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.GetAll;

public sealed class GetAllWorkspacesQueryHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetAllWorkspacesQuery, Result<List<WorkspaceSummaryDto>>>
{
    public async Task<Result<List<WorkspaceSummaryDto>>> Handle(
        GetAllWorkspacesQuery request,
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

        var workspaces = await uow.WorkspaceRepository.GetAllAsync();

        return workspaces is null
            ? Error.Unknown
            : workspaces.Select(workspace => workspace.ToSummaryDto()).ToList();
    }
}
