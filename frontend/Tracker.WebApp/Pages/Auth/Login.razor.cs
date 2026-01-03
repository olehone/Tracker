using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Requests;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Models;
using Tracker.WebApp.Shared;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages.Auth;

public partial class Login
{
    [Inject]
    private IAuthService AuthService { get; set; } = null!;

    [Inject]
    private IUserService UserService { get; set; } = null!;

    [Inject]
    private NavigationManager Navigation { get; set; } = null!;

    [CascadingParameter]
    private AppState? AppState { get; set; }

    private LoginUserModel loginModel = new();
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

        if (UiHelper.IsEmailInvalid(loginModel.Email))
        {
            errorMessages = ["Wrong email format"];
            return;
        }

        isLoading = true;

        try
        {
            await AuthService.LoginAsync(ToRequest(loginModel));

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

    private static LoginUserRequest ToRequest(LoginUserModel model)
    {
        return new LoginUserRequest()
        {
            Email = model.Email,
            Password = model.Password
        };
    }
}