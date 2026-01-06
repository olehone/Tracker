using Tracker.Domain.Enums;

namespace Tracker.Domain.ValueObjects;

public class BoardSettings
{
    public BoardVisibility Visibility { get; set; } 
        = BoardVisibility.Private;
    public BoardPermissionRole MinCreateItemRole { get; set; } 
        = BoardPermissionRole.Member;
    public BoardPermissionRole MinChangeItemRole { get; set; } 
        = BoardPermissionRole.Member;
    public BoardPermissionRole MinCreateListRole { get; set; } 
        = BoardPermissionRole.Admin;
    public BoardPermissionRole MinChangeListRole { get; set; } 
        = BoardPermissionRole.Admin;
}