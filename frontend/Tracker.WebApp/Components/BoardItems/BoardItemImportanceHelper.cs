using MudBlazor;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.BoardItems;

internal static class BoardItemImportanceHelper
{
    public static Color GetImportanceColor(BoardItemImportance importance)
    {
        return importance switch
        {
            BoardItemImportance.Low => Color.Default,
            BoardItemImportance.Medium => Color.Info,
            BoardItemImportance.High => Color.Warning,
            _ => Color.Error
        };
    }

    public static string GetImportanceIcon(BoardItemImportance importance)
    {
        return importance switch
        {
            BoardItemImportance.Low => @Icons.Material.Outlined.KeyboardArrowDown,
            BoardItemImportance.Medium => @Icons.Material.Outlined.HorizontalRule,
            BoardItemImportance.High => @Icons.Material.Outlined.KeyboardArrowUp,
            _ => @Icons.Material.Filled.Error
        };
    }
}