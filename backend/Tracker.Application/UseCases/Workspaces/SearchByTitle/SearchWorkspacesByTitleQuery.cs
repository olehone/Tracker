using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Workspaces.SearchByTitle;

public class SearchWorkspacesByTitleQuery : IRequest<Result<List<WorkspaceSummaryDto>>>
{
    public required string Title { get; set; }
    public required int Page { get; set; } = 1;
    public required int AmountInPage { get; set; } = 20;
}
