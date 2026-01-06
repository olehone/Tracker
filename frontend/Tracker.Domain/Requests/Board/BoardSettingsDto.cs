using Tracker.Domain.Enums;

namespace Tracker.Domain.Requests.Board;

public class ChangeBoardSettingsRequest
{
    public required Guid BoardId { get; set; }
    public required string Title { get; set; }
    public required string? Description { get; set; }
    public required BoardVisibility Visibility { get; set; } 
    public required BoardPermissionRole MinCreateItemRole { get; set; } 
    public required BoardPermissionRole MinChangeItemRole { get; set; } 
    public required BoardPermissionRole MinCreateListRole { get; set; } 
    public required BoardPermissionRole MinChangeListRole { get; set; } 
}