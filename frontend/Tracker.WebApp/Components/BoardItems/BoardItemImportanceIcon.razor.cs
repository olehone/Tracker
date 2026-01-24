using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.BoardItems;

public partial class BoardItemImportanceIcon
{
    [Parameter, EditorRequired]
    public BoardItemImportance Importance { get; set; }
    [Parameter]
    public bool HasAttention { get; set; } = false;

    private string GetIcon()
    {
        return BoardItemImportanceHelper
            .GetImportanceIcon(Importance);
    }

    private Color GetColor()
    {
        return BoardItemImportanceHelper
            .GetImportanceColor(Importance);
    }

    private Variant GetVariant()
    {
        return HasAttention
            ? Variant.Filled
            : Variant.Outlined;
    }
}