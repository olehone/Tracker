using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;

namespace Tracker.Domain.Mapping;

public static class UserMapping
{
    public static UserDto ToDto(this User user, string? avatarUrl = null)
    {
        return new UserDto()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            Role = user.Role,
            FirstName = user.FirstName,
            LastName = user.LastName ?? "",
            AvatarUrl = user.GetAvatar(avatarUrl)
        };
    }

    public static string? GetAvatar(this User user, string? avatarUrl)
    {
        if (user.AvatarUpdatedAt is null)
        {
            return null;
        }
        return $"{avatarUrl}?v={user.AvatarUpdatedAt.Value.Ticks}";
    }
}