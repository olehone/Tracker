using Refit;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.BoardItem;
using Tracker.Domain.Requests.BoardList;

namespace Tracker.Services.Abstraction;

public interface IBoardListService
{
    public Task<BoardListDto> CreateBoardListAsync(CreateBoardListRequest request);
    public Task<ApiResponse<object>> MoveBoardListAsync(MoveBoardListRequest request);
}