using Tracker.Domain.Enums;

namespace Tracker.Domain.Dtos;

public class UserPermissionsDto
{
    public required SubscriptionPlan CurrentPlan { get; set; }
    public required bool CanSeeBoardCalendar { get; set; }
    public required bool CanSeeBoardEisenhower { get; set; }
    public required bool CanUseAi { get; set; }
}
