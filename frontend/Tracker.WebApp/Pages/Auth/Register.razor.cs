using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Requests;
using Tracker.Services.Abstraction;
using Tracker.Services.States;

namespace Tracker.WebApp.Pages.Auth;
public partial class Register
{
    [Inject] private IAuthService Auth { get; set; } = default!;
    [Inject] private IAuthStateNotifier Notifier { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [CascadingParameter] private AppState? AppState { get; set; }

    private RegisterUserRequest registerModel = new();
    private string? errorMessage;
    private string? secondPassword;
    private bool isLoading = false;
    private bool isSuccess = false;
    private MudForm form;

    private async Task HandleRegister()
    {
        await form!.Validate();

        if (!form.IsValid)
        {
            return;
        }
        if (registerModel.Password!= secondPassword)
        {
            errorMessage = "Passwords not the same";
            return;
        }
        isLoading = true;
        errorMessage = null;

        try
        {
            await Auth.RegisterAsync(registerModel);

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
            errorMessage = ex.Message;
            Console.WriteLine(ex);
        }
        finally
        {
            isLoading = false;
        }
    }
}