using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;

namespace Tracker.Domain.Mapping;

public static class UserMapping
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto()
        {
            Id = user.Id,
            Email = user.Email,
            Username = user.Username,
            Role = user.Role,
            FirstName = user.FirstName,
            LastName = user.LastName ?? "",
        };
    }
}