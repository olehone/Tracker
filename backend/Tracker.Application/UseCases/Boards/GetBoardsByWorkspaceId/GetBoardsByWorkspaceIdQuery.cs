using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.GetBoardsByWorkspaceId;

public class GetBoardsByWorkspaceIdQuery : IRequest<Result<List<BoardSummaryDto>>>
{
    public Guid WorkspaceId { get; set; }
}

