using System.Runtime.InteropServices;
using Tracker.Domain.Dtos;
using Tracker.Domain.Requests.Board;

namespace Tracker.Services.Abstraction;

public interface IBoardService
{
    Task<BoardFullDto> GetBoardByIdAsync(Guid id);
    Task<BoardSummaryDto> CreateBoardAsync(CreateBoardRequest request);
}

