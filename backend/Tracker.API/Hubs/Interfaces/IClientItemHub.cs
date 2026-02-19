using Tracker.API.Hubs.Events;

namespace Tracker.API.Hubs.Interfaces;

public interface IClientItemHub
{
    Task CommentCreated(CommentCreatedEvent evt);
    Task CommentUpdated(CommentUpdatedEvent evt);
    Task CommentDeleted(CommentDeletedEvent evt);
}