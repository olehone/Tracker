using MudBlazor;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.BoardItems;

internal static class ImportanceHelper
{
    public static Color GetColor(BoardItemImportance importance)
    {
        return importance switch
        {
            BoardItemImportance.Low => Color.Default,
            BoardItemImportance.Medium => Color.Info,
            BoardItemImportance.High => Color.Warning,
            _ => Color.Error
        };
    }

    public static string GetIcon(BoardItemImportance importance)
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