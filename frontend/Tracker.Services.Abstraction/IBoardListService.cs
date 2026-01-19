using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardListService
{
    public Task<Result<BoardListDto>> CreateAsync(Guid boardId,
        CreateBoardListRequest request);
    public Task<Result> MoveAsync(Guid id, MoveBoardListRequest request);
    public Task<Result> UpdateAsync(Guid id, UpdateBoardListRequest request);
    public Task<Result> DeleteAsync(Guid id);
}
