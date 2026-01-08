using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardSummary
{
    [Parameter]
    public required BoardSummaryDto Board { get; set; }

    private static string GetBoardColorBackgroundStyle(BoardSummaryDto board)
    {
        return UiHelper.GetColorByString(board.Id.ToString());
    }
}