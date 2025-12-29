using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Requests;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages.Auth;
public partial class Login
{
    [Inject] private IAuthService AuthService { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [CascadingParameter] private AppState? AppState { get; set; }

    private LoginUserRequest loginModel = new();
    private IReadOnlyList<string>? errorMessages = [];
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
        if (IsEmailInvalid(loginModel.Email))
        {
            errorMessages = ["Wrong email format"];
            return;
        }
        isLoading = true;

        try
        {
            await AuthService.LoginAsync(loginModel);

            var currentUser = await UserService.GetCurrentUserAsync();
            if (AppState != null && currentUser != null)
            {
                AppState.CurrentUser = currentUser;
            }

            Navigation.NavigateTo("/", forceLoad: false);
        }
        catch (HttpRequestException)
        {
            errorMessages = ["Unable to connect to the server. Please try again."];
        }
        catch (Refit.ValidationApiException ex)
        {
            if (ex.Content is null)
            {
                errorMessages = ["Unknown error from server"];
                return;
            }
            if (ex.Content.Errors.Count > 0)
            {
                errorMessages = ex.Content.Errors.SelectMany(error => error.Value).ToList();
            }
            else
            {
                var title = ex.Content.Title ?? "Unknown error from server";
                errorMessages = [title];
            }
        }
        catch (Exception)
        {
            errorMessages = ["Invalid email or password. Please try again."];
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    private bool IsEmailInvalid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return true;
        }
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address != email;
        }
        catch
        {
            return true;
        }

    }
}