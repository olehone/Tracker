using Microsoft.AspNetCore.Components;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages.Subscription;

public partial class SubscriptionSuccess
{
    [Inject]
    public AppState AppState { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await AppState.ReloadAsync();
    }
}