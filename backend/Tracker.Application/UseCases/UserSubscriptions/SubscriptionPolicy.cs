using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;

namespace Tracker.Application.UseCases.UserSubscriptions;

public static class SubscriptionPolicy
{
    public static UserPermissionsDto GetPermissions(
        GlobalRole role,
        SubscriptionPlan? subscriptionPlan)
    {
        if (role >= GlobalRole.Admin)
        {
            var allPlan = All();
            allPlan.CurrentPlan = subscriptionPlan ?? SubscriptionPlan.Free;
            return allPlan;
        }

        var plan = subscriptionPlan ?? SubscriptionPlan.Free;

        return new UserPermissionsDto
        {
            CurrentPlan = plan,
            CanSeeBoardCalendar = plan >= SubscriptionPlan.Basic,
            CanSeeBoardEisenhower = plan >= SubscriptionPlan.Pro,
            CanSeeBoardRoadmap = plan >= SubscriptionPlan.Basic,
            CanUseAi = CanUseAi(plan),
        };
    }

    public static bool CanUseAi(SubscriptionPlan plan)
    {
        return plan >= SubscriptionPlan.Basic;
    }

    public static UserPermissionsDto None => new()
    {
        CurrentPlan = SubscriptionPlan.Free,
        CanSeeBoardCalendar = false,
        CanSeeBoardEisenhower = false,
        CanSeeBoardRoadmap = false,
        CanUseAi = false,
    };

    public static UserPermissionsDto All()
    {
        return new()
        {
            CurrentPlan = SubscriptionPlan.Free,
            CanSeeBoardCalendar = true,
            CanSeeBoardEisenhower = true,
            CanSeeBoardRoadmap = true,
            CanUseAi = true,
        };
    }
}