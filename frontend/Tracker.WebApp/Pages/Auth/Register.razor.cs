using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Requests;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages.Auth;
public partial class Register
{
    [Inject] private IAuthService Auth { get; set; } = default!;
    [Inject] private IUserService UserService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    [CascadingParameter] private AppState? AppState { get; set; }

    private RegisterUserRequest registerModel = new();
    private IReadOnlyList<string>? errorMessages;
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
        if (registerModel.Password != secondPassword)
        {
            errorMessages = ["Passwords are not the same"];
            return;
        }
        if (IsEmailInvalid(registerModel.Email))
        {
            errorMessages = ["Wrong email format"];
            return;
        }
        isLoading = true;

        try
        {
            await Auth.RegisterAsync(registerModel);

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
            StateHasChanged();
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
            StateHasChanged();
        }
        catch (Exception)
        {
            errorMessages = ["Invalid email or password. Please try again."];
            StateHasChanged();
        }
        finally
        {
            isLoading = false;
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