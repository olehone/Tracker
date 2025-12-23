using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Dtos;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.GetBoardsByWorkspaceId;

public class GetBoardsByWorkspaceIdQueryHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<GetBoardsByWorkspaceIdQuery, Result<List<BoardSummaryDto>>>
{
    public async Task<Result<List<BoardSummaryDto>>> Handle(
        GetBoardsByWorkspaceIdQuery query, 
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var boards = await uow.BoardRepository
            .GetAllByWorkspaceId(query.WorkspaceId);

        return boards is null
            ? Error.NotFound("Workspace")
            : boards.Select(b=> b.ToSummaryDto()).ToList();
    }
}
