using MudBlazor;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.Boards;

public static class ArchiveStatusHelper
{
    public static string GetIcon(ArchiveStatus status)
    {
        return status switch
        {
            ArchiveStatus.None => Icons.Material.Outlined.HelpOutline,
            ArchiveStatus.NotArchived => Icons.Material.Outlined.CheckCircleOutline,
            ArchiveStatus.Pending => Icons.Material.Outlined.Schedule,
            ArchiveStatus.Queued => Icons.Material.Outlined.Queue,
            ArchiveStatus.Completed => Icons.Material.Outlined.DoneAll,
            ArchiveStatus.Failed => Icons.Material.Outlined.ErrorOutline,
            _ => Icons.Material.Outlined.HelpOutline
        };
    }

    public static Color GetColor(ArchiveStatus status)
    {
        return status switch
        {
            ArchiveStatus.None => Color.Default,
            ArchiveStatus.NotArchived => Color.Primary,
            ArchiveStatus.Pending => Color.Info,
            ArchiveStatus.Queued => Color.Warning,
            ArchiveStatus.Completed => Color.Success,
            ArchiveStatus.Failed => Color.Error,
            _ => Color.Default
        };
    }
}
