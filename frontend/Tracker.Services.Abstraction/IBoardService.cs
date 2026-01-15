using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardService
{
    Task<Result<BoardFullDto>> GetBoardByIdAsync(Guid id);
    Task<Result> UpdateAsync(Guid id, UpdateBoardRequest request);
    Task<Result<BoardSummaryDto>> CreateBoardAsync(CreateBoardRequest request);
}