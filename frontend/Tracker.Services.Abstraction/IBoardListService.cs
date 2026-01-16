using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardListService
{
    public Task<Result<BoardListDto>> CreateBoardListAsync(Guid boardId,
        CreateBoardListRequest request);
    public Task<Result> MoveBoardListAsync(Guid id, MoveBoardListRequest request);
    public Task<Result> UpdateBoardListAsync(Guid id, UpdateBoardListRequest request);
    public Task<Result> DeleteBoardListAsync(Guid id);
}
