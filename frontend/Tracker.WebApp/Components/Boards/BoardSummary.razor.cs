using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Components.Boards;
public partial class BoardSummary
{
    [Parameter]
    public required BoardSummaryDto Board { get; set; }

    private static string GetBoardColorBackgroundStyle(BoardSummaryDto board)
    {
        var hash = board.Id.GetHashCode();
        var hue = Math.Abs(hash % 360);
        return $"background:hsl({hue}, 60%, 55%);";
    }
}