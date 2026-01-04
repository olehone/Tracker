namespace Tracker.Domain.Enums;

public enum WorkspacePermissionRole
{
    Any = 1,
    Observer = 10,
    Member = 20,
    Admin = 30,
    Owner = 40,
    None = 100
}