using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction;

public interface IBoardService
{
    Task<Result<BoardFullDto>> GetByIdAsync(Guid id);
    Task<Result<BoardSummaryDto>> CreateAsync(CreateBoardRequest request);
    Task<Result> UpdateAsync(Guid id, UpdateBoardRequest request);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<List<BoardSummaryDto>>> GetForCurrentUserAsync();
}