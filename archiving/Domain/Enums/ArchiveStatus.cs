namespace Domain.Enums;

public enum ArchiveStatus
{
    None = 0,
    NotArchived = 10,
    PendingArchive = 20,
    QueuedArchive = 30,
    Archived = 40,
    PendingUnarchive = 50,
    QueuedUnarchive = 60,
    Failed = 70,
}