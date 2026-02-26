namespace Domain.Enums;

public enum BoardPermissionRole
{
    Any = 1,
    WorkspaceMember = 10,
    Observer = 20,
    Member = 30,
    Admin = 40,
    Owner = 50,
    None = 100,
}