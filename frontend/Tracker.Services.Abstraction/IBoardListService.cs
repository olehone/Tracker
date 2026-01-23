using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardListService
{
    Task<Result<BoardListDto>> CreateAsync(Guid boardId, string title);
    Task<Result> MoveAsync(Guid boardId, Guid itemId, MoveBoardListRequest request);
    Task<Result> UpdateAsync(Guid boardId, Guid itemId, UpdateBoardListRequest request);
    Task<Result> DeleteAsync(Guid boardId, Guid itemId);
}
