using MediatR;
using Tracker.Application.Common.Auth;
using Tracker.Application.Common.Repositories;
using Tracker.Application.Common.UnitOfWork;
using Tracker.Domain.Entities;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.StartCall;

public class StartBoardCallCommandHandler(
    IBoardCallRepository boardCallRepository,
    ICallRepository callRepository,
    IUserContext userContext,
    IUnitOfWorkFactory unitOfWorkFactory)
    : IRequestHandler<StartBoardCallCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(StartBoardCallCommand request,
        CancellationToken cancellationToken)
    {
        await using var uow = unitOfWorkFactory.Create();
        var boardResult = await BoardHelper.GetBoardForActionAsync(uow, userContext, request.BoardId, BoardAction.ChangeBoard);

        if (boardResult.IsFailure)
        {
            return boardResult.Error;
        }
        var board = boardResult.Value;
        Call? call;

        var callId = await boardCallRepository.GetCallIdAsync(board.Id);

        if (callId.HasValue)
        {
            call = await callRepository.GetCallByIdAsync(callId.Value);
            if (call is not null)
            {
                return call.Id;
            }
        }

        var newCallId = Guid.NewGuid();

        await boardCallRepository.AddCallAsync(board.Id, newCallId);

        call = new Call
        {
            Id = newCallId,
            StartedAt = DateTimeOffset.UtcNow,
            Users = []
        };

        await callRepository.SaveCallAsync(call);

        return newCallId;
    }
}