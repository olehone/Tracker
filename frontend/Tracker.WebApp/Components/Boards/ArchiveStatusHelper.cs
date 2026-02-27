using MudBlazor;
using Tracker.Domain.Enums;

namespace Tracker.WebApp.Components.Boards;

public static class ArchiveStatusHelper
{
    public static string GetIcon(ArchiveStatus status)
    {
        return status switch
        {
            ArchiveStatus.NotArchived => Icons.Material.Outlined.CheckCircleOutline,
            ArchiveStatus.Archived => Icons.Material.Filled.Archive,

            ArchiveStatus.PendingArchive or
            ArchiveStatus.QueuedArchive or
            ArchiveStatus.PendingUnarchive or
            ArchiveStatus.QueuedUnarchive
                => Icons.Material.Filled.LockClock,

            ArchiveStatus.None or
            ArchiveStatus.Failed or
            _ => Icons.Material.Outlined.ErrorOutline
        };
    }

    public static Color GetColor(ArchiveStatus status)
    {
        return status switch
        {
            ArchiveStatus.NotArchived => Color.Primary,
            ArchiveStatus.Archived => Color.Error,

            ArchiveStatus.PendingArchive or
            ArchiveStatus.QueuedArchive or
            ArchiveStatus.PendingUnarchive or
            ArchiveStatus.QueuedUnarchive
                => Color.Warning,

            ArchiveStatus.None or
            ArchiveStatus.Failed or
            _ => Color.Dark
        };
    }

    public static string GetDescription(ArchiveStatus status)
    {
        return status switch
        {
            ArchiveStatus.NotArchived => "Not archived",
            ArchiveStatus.Archived => "Archived",

            ArchiveStatus.PendingArchive => "Scheduled to archive",
            ArchiveStatus.QueuedArchive => "Archiving..",

            ArchiveStatus.PendingUnarchive => "Scheduled to unarchive",
            ArchiveStatus.QueuedUnarchive => "Unarchiving..",

            ArchiveStatus.None or
            ArchiveStatus.Failed or
            _ => "Something went wrong"
        };
    }
}
