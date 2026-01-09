using Microsoft.AspNetCore.Components;
using Tracker.Domain.Dtos;
using Tracker.WebApp.Shared;

namespace Tracker.WebApp.Components.Boards;

public partial class BoardSummary
{
    [Parameter]
    public required BoardSummaryDto Board { get; set; }
    private string? _customColor;
    private string CustomColor
    {
        get
        {
            _customColor ??= UiHelper.GetColorByString(Board.Id);
            return _customColor;
        }
    }
}