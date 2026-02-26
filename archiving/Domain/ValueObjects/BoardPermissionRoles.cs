using Domain.Enums;

namespace Domain.ValueObjects;

public class BoardPermissionRoles
{
    public BoardPermissionRole MinCreateItemRole { get; set; }
        = BoardPermissionRole.Member;
    public BoardPermissionRole MinChangeItemRole { get; set; }
        = BoardPermissionRole.Member;
    public BoardPermissionRole MinCreateListRole { get; set; }
        = BoardPermissionRole.Admin;
    public BoardPermissionRole MinChangeListRole { get; set; }
        = BoardPermissionRole.Admin;
}