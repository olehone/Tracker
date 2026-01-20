using MediatR;
using Tracker.Domain.Dtos;
using Tracker.Domain.Results;

namespace Tracker.Application.UseCases.Boards.GetForCurrentUser;

public class GetBoardsForCurrentUserQuery : IRequest<Result<List<BoardSummaryDto>>>
{
}
