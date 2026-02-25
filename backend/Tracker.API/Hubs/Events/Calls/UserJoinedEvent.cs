using Tracker.Domain.Dtos;

namespace Tracker.API.Hubs.Events.Calls;

public record UserJoinedEvent(UserDto User);