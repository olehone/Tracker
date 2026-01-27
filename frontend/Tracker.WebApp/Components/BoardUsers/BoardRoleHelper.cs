using MudBlazor;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.BoardUsers;

internal static class BoardRoleHelper
{
    public static Color GetRoleColor(BoardUserRole role)
    {
        return role switch
        {
            BoardUserRole.Owner => Color.Warning,
            BoardUserRole.Admin => Color.Error,
            BoardUserRole.Member => Color.Primary,
            BoardUserRole.Observer => Color.Info,
            _ => Color.Default
        };
    }

    public static string GetRoleIcon(BoardUserRole role)
    {
        return role switch
        {
            BoardUserRole.Owner => Icons.Material.Filled.Star,
            BoardUserRole.Admin => Icons.Material.Filled.AdminPanelSettings,
            BoardUserRole.Member => Icons.Material.Filled.Person,
            BoardUserRole.Observer => Icons.Material.Filled.Visibility,
            _ => Icons.Material.Filled.PersonOff
        };
    }
}