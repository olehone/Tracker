using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Requests;
using Tracker.Services.Abstraction;
using Tracker.Services.States;

namespace Tracker.WebApp.Pages.Auth;
public partial class Login
{
    [Inject] private IAuthService Auth { get; set; } = default!;
    [Inject] private IAuthStateNotifier Notifier { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [CascadingParameter] private AppState? AppState { get; set; }

    private LoginUserRequest loginModel = new();
    private string? errorMessage;
    private bool isLoading = false;
    private bool isSuccess = false;
    private MudForm form;

    private async Task HandleLogin()
    {
        await form!.Validate();

        if (!form.IsValid)
        {
            return;
        }

        isLoading = true;
        errorMessage = null;

        try
        {
            await Auth.LoginAsync(loginModel);

            var currentUser = await Auth.GetCurrentUserAsync();
            if (AppState != null && currentUser != null)
            {
                AppState.CurrentUser = currentUser;
            }

            Notifier.NotifyUserAuthentication();
            Navigation.NavigateTo("/", forceLoad: false);
        }
        catch (HttpRequestException)
        {
            errorMessage = "Unable to connect to the server. Please try again.";
        }
        catch (Exception ex)
        {
            errorMessage = "Invalid email or password. Please try again.";
        }
        finally
        {
            isLoading = false;
        }
    }
}