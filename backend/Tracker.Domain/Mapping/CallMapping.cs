using Tracker.Domain.Dtos;
using Tracker.Domain.Entities;

namespace Tracker.Domain.Mapping;

public static class CallMapping
{
    public static CallDto ToDto(this Call call)
    {
        return new CallDto()
        {
            Id = call.Id,
            StartedAt = call.StartedAt,
            Users = call.Users.Select(u => u.User).ToList(),
        };
    }
}