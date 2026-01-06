using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardList;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardListService
{
    public Task<Result<BoardListDto>> CreateBoardListAsync(CreateBoardListRequest request);
    public Task<Result> MoveBoardListAsync(MoveBoardListRequest request);
}