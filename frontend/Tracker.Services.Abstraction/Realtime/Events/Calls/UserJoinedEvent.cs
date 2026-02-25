using Tracker.Domain.Dtos;

namespace Tracker.Services.Abstraction.Realtime.Events.Calls;

public record UserJoinedEvent(UserDto User);
