using Tracker.Domain.Enums;

namespace Tracker.Domain.Entities;

public class BoardSettings
{
    public BoardVisibility Visibility { get; set; } 
        = BoardVisibility.Private;
    public BoardPermissionRole MinCreateItemRole { get; set; } 
        = BoardPermissionRole.Member;
    public BoardPermissionRole MinMoveItemRole { get; set; } 
        = BoardPermissionRole.Member;
    public BoardPermissionRole MinCreateListRole { get; set; } 
        = BoardPermissionRole.Admin;
    public BoardPermissionRole MinMoveListRole { get; set; } 
        = BoardPermissionRole.Admin;

}