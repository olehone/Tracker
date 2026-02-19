using Microsoft.AspNetCore.SignalR.Client;
using Tracker.Domain.Events;
using Tracker.Services.Abstraction;
using Tracker.Services.Abstraction.Realtime;
using Tracker.Services.Realtime.Methods;

namespace Tracker.Services.Realtime;

public class BoardRealtimeService(IApiUrlService apiUrl, IAuthService authService) 
    : RealtimeService(apiUrl, authService, "hubs/board"), IBoardRealtimeService
{
    public event Action<ItemCreatedEvent>? OnItemCreated;
    public event Action<ItemMovedEvent>? OnItemMoved;
    public event Action<ItemUpdatedEvent>? OnItemUpdated;
    public event Action<ItemDeletedEvent>? OnItemDeleted;

    public event Action<ListCreatedEvent>? OnListCreated;
    public event Action<ListMovedEvent>? OnListMoved;
    public event Action<ListUpdatedEvent>? OnListUpdated;
    public event Action<ListDeletedEvent>? OnListDeleted;

    public override ValueTask DisposeAsync()
    {
        OnItemCreated = null;
        OnItemMoved = null;
        OnItemUpdated = null;
        OnItemDeleted = null;

        OnListCreated = null;
        OnListMoved = null;
        OnListUpdated = null;
        OnListDeleted = null;

        return base.DisposeAsync();
    }

    public override void RegisterEvents(HubConnection connection)
    {
        connection!.On<ItemCreatedEvent>(BoardRealtimeMethods.ItemCreated, (evt) =>
        {
            OnItemCreated?.Invoke(evt);
        });

        connection!.On<ItemMovedEvent>(BoardRealtimeMethods.ItemMoved, (evt) =>
        {
            OnItemMoved?.Invoke(evt);
        });

        connection!.On<ItemUpdatedEvent>(BoardRealtimeMethods.ItemUpdated, (evt) =>
        {
            OnItemUpdated?.Invoke(evt);
        });

        connection!.On<ItemDeletedEvent>(BoardRealtimeMethods.ItemDeleted, (evt) =>
        {
            OnItemDeleted?.Invoke(evt);
        });

        connection!.On<ListCreatedEvent>(BoardRealtimeMethods.ListCreated, (evt) =>
        {
            OnListCreated?.Invoke(evt);
        });
        connection!.On<ListMovedEvent>(BoardRealtimeMethods.ListMoved, (evt) =>
        {
            OnListMoved?.Invoke(evt);
        });
        connection!.On<ListUpdatedEvent>(BoardRealtimeMethods.ListUpdated, (evt) =>
        {
            OnListUpdated?.Invoke(evt);
        });
        connection!.On<ListDeletedEvent>(BoardRealtimeMethods.ListDeleted, (evt) =>
        {
            OnListDeleted?.Invoke(evt);
        });
    }
}