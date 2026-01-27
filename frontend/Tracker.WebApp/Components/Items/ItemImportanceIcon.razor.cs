using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.Items;

public partial class ItemImportanceIcon
{
    [Parameter, EditorRequired]
    public BoardItemImportance Importance { get; set; }
    [Parameter]
    public bool HasAttention { get; set; } = false;

    private string GetIcon()
    {
        return ImportanceHelper
            .GetIcon(Importance);
    }

    private Color GetColor()
    {
        return ImportanceHelper
            .GetColor(Importance);
    }

    private Variant GetVariant()
    {
        return HasAttention
            ? Variant.Filled
            : Variant.Outlined;
    }
}