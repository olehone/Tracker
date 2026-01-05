using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;
using Tracker.Domain.Requests.Common;
using Tracker.Domain.Results;

namespace Tracker.Services.Abstraction.Entities;

public interface IBoardService
{
    Task<Result<BoardFullDto>> GetBoardByIdAsync(GetByIdRequest request);
    Task<Result<BoardSummaryDto>> CreateBoardAsync(CreateBoardRequest request);
}