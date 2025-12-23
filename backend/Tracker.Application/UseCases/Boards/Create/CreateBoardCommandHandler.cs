using MediatR;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Application.UseCases.Auth.Register;
using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;
using Tracker.Domain.Mapping;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.AddNewBoard;

public sealed class CreateBoardCommandHandler(
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<CreateBoardCommand, Result<BoardSummaryDto>>
{
    public async Task<Result<BoardSummaryDto>> Handle(
        CreateBoardCommand command,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();

        var workspace = await uow.WorkspaceRepository.GetByIdAsync(command.WorkspaceId);
        
        if (workspace is null)
        {
            return Error.NotFound("Workspace");
        }

        var board = new Board()
        {
            WorkspaceId = command.WorkspaceId,
            Title = command.Title,
            Description = command.Description
        };
        await uow.BoardRepository.AddAsync(board);

        var sc = await uow.SaveChangesAsync(cancellationToken);

        return sc.IsFailure
            ? Error.Unknown
            : board.ToSummaryDto();
    }
}
