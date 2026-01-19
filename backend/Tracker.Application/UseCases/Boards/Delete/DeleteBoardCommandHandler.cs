using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.Delete;

public class DeleteBoardCommandHandler(
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<DeleteBoardCommand, Result>
{
    public async Task<Result> Handle(
        DeleteBoardCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();
        var boardResult = await BoardHelper.GetBoardForActionAsync(uow, userContext, request.BoardId, BoardAction.ChangeBoard);

        if (boardResult.IsFailure)
        {
            return boardResult.Error;
        }
        var board = boardResult.Value;

        await uow.BoardRepository.RemoveAsync(board.Id);
        
        var sc = await uow.SaveChangesAsync(cancellationToken);
        return sc.IsFailure
            ? Error.Unknown
            : Result.Success();
    }
}