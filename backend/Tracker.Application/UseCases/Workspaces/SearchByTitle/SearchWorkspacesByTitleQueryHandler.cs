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
    : IRequestHandler<SearchWorkspacesByTitleQuery, Result<List<WorkspaceSummaryDto>>>
{
    public async Task<Result<List<WorkspaceSummaryDto>>> Handle(
        SearchWorkspacesByTitleQuery request,
        CancellationToken cancellationToken)
    {
        if (userContext.IsUnauthenticated())
        {
            return AuthErrors.Unauthenticated;
        }
        var userId = userContext.GetUserId();

        await using var uow = unitOfWorkFactory.Create();

        int skip = (request.Page - 1) * request.AmountInPage;
        var workspaces = await uow.WorkspaceRepository
            .GetAllAsync(skip, request.AmountInPage, request.Title, userId);

        return workspaces is null
            ? Error.NotFound("Workspaces", "title")
            : workspaces.Select(workspace => workspace.ToSummaryDto()).ToList();
    }
}
