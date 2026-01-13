using MudBlazor;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.BoardUsers;

internal static class BoardRoleHelper
{
    public static Color GetRoleColor(UserBoardRole role)
    {
        return role switch
        {
            UserBoardRole.Owner => Color.Warning,
            UserBoardRole.Admin => Color.Error,
            UserBoardRole.Member => Color.Primary,
            UserBoardRole.Observer => Color.Info,
            _ => Color.Default
        };
    }

    public static string GetRoleIcon(UserBoardRole role)
    {
        return role switch
        {
            UserBoardRole.Owner => Icons.Material.Filled.Star,
            UserBoardRole.Admin => Icons.Material.Filled.AdminPanelSettings,
            UserBoardRole.Member => Icons.Material.Filled.Person,
            UserBoardRole.Observer => Icons.Material.Filled.Visibility,
            _ => Icons.Material.Filled.PersonOff
        };
    }
}