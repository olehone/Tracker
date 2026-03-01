using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Enums;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages.Subscription;

public partial class Subscription
{
    [CascadingParameter]
    public AppState AppState { get; set; } = null!;

    [Inject] private ISubscriptionService SubscriptionService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;


    private async Task SelectPlanAsync(SubscriptionPlan plan)
    {
        var result = await SubscriptionService.GetCheckoutUrlAsync(plan);
        if (result.IsFailure)
        {
            return;
        }
        Navigation.NavigateTo(result.Value, forceLoad: true);
    }

    private async Task StopSubscription()
    {
        var result = await SubscriptionService.StopSubscriptionAsync();
        if (result.IsSuccess)
        {
            Snackbar.Add("Successfully stopped your subscription", Severity.Success);
        }
        else
        {
            Snackbar.Add("Cannot stop your subscription right now, try later", Severity.Error);
        }
        await AppState.ReloadAsync();
    }
}