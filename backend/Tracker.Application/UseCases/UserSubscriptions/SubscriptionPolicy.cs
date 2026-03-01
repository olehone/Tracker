using Tracker.Domain.Dtos;
using Tracker.Domain.Enums;

namespace Tracker.Application.UseCases.UserSubscriptions;

public static class SubscriptionPolicy
{
    public static UserPermissionsDto GetPermissions(
        GlobalRole role,
        SubscriptionPlan? subscriptionPlan,
        int aiQueriesUsed)
    {
        if (role >= GlobalRole.Admin)
        {
            return All(aiQueriesUsed);
        }

        var plan = subscriptionPlan ?? SubscriptionPlan.Free;

        var aiLimit = GetAiQueriesLimit(plan);

        return new UserPermissionsDto
        {
            CurrentPlan = plan,
            CanSeeBoardCalendar = plan >= SubscriptionPlan.Basic,
            CanSeeBoardEisenhower = plan >= SubscriptionPlan.Pro,
            CanUseAi = CanUseAi(plan, aiQueriesUsed),
            IsAiLimited = plan <= SubscriptionPlan.Pro,
            AiQueriesLimit = aiLimit is null
                ? null
                : aiLimit,
            AiQueriesUsed = aiQueriesUsed,
            AiQueriesRemaining = aiLimit is null
                ? null
                : Math.Max(0, aiLimit.Value - aiQueriesUsed),
        };
    }

    public static bool CanUseAi(SubscriptionPlan plan, int aiQueriesUsed)
    {
        if (plan < SubscriptionPlan.Basic)
        {
            return false;
        }

        var limit = GetAiQueriesLimit(plan);
        if (limit is null)
        {
            return true;
        }

        return aiQueriesUsed < limit;
    }

    public static int? GetAiQueriesLimit(SubscriptionPlan plan)
    {
        return plan switch
        {
            SubscriptionPlan.Free => 0,
            SubscriptionPlan.Basic => 10,
            SubscriptionPlan.Pro => null,
            _ => 0
        };
    }

    public static UserPermissionsDto None => new()
    {
        CurrentPlan = SubscriptionPlan.Free,
        CanSeeBoardCalendar = false,
        CanSeeBoardEisenhower = false,
        CanUseAi = false,
        IsAiLimited = true,
        AiQueriesUsed = 0,
    };

    public static UserPermissionsDto All(int aiQueriesUsed)
    {
        return new()
        {
            CurrentPlan = SubscriptionPlan.Pro,
            CanSeeBoardCalendar = true,
            CanSeeBoardEisenhower = true,
            CanUseAi = true,
            IsAiLimited = false,
            AiQueriesUsed = aiQueriesUsed,
        };
    }
}