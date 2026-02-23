using Microsoft.AspNetCore.SignalR.Client;
using Tracker.Services.Abstraction.Realtime.Events;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Realtime.Methods;
using Tracker.Services.Abstraction.Realtime.Events;

namespace Tracker.Services.Realtime;

public class ItemRealtimeService(IApiUrlService apiUrl,IAuthService authService) 
    : RealtimeService(apiUrl, authService, "hubs/item"), IItemRealtimeService
{

    public event Action<CommentCreatedEvent>? OnCommentCreated;
    public event Action<CommentUpdatedEvent>? OnCommentUpdated;
    public event Action<CommentDeletedEvent>? OnCommentDeleted;

    public override ValueTask DisposeAsync()
    {
        OnCommentCreated = null;
        OnCommentUpdated = null;
        OnCommentDeleted = null;

        return base.DisposeAsync();
    }

    public override void RegisterEvents(HubConnection connection)
    {
        connection!.On<CommentCreatedEvent>(ItemRealtimeMethods.CommentCreated, (evt) =>
        {
            OnCommentCreated?.Invoke(evt);
        });

        connection!.On<CommentUpdatedEvent>(ItemRealtimeMethods.CommentUpdated, (evt) =>
        {
            OnCommentUpdated?.Invoke(evt);
        });

        connection!.On<CommentDeletedEvent>(ItemRealtimeMethods.CommentDeleted, (evt) =>
        {
            OnCommentDeleted?.Invoke(evt);
        });
    }
}