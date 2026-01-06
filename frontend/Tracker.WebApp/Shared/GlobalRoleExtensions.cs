using Tracker.Domain.Dtos;

namespace Tracker.WebApp.Shared;

public static class GlobalRoleExtensions
{
    public static GlobalRole ToGlobalRole(this string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return GlobalRole.None;
        }

        return Enum.TryParse(role, true, out GlobalRole value)
            ? value
            : GlobalRole.None;
    }

    public static string WithHigher(this string? role)
    {
        var current = role.ToGlobalRole();

        return current.WithHigher();
    }

    public static string WithHigher(this GlobalRole role)
    {
        var roles = Enum.GetValues<GlobalRole>()
            .Where(gr => (int)gr >= (int)role)
            .OrderBy(gr => (int)gr)
            .Select(gr => gr.ToString());

        return string.Join(", ", roles);
    }
}