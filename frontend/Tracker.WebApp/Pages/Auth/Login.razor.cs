using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Requests;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Entities;
using Tracker.WebApp.Models;
using Tracker.WebApp.Shared;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages.Auth;

public partial class Login
{
    [CascadingParameter]
    private AppState? AppState { get; set; }

    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private IUserService UserService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;


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

        var result = await AuthService.LoginAsync(ToRequest(loginModel));
        if (NotifyIfError(result))
        {
            return;
        }

        var userResult = await UserService.GetCurrentUserAsync();
        if (NotifyIfError(userResult))
        {
            return;
        }
        if (AppState != null && userResult.IsSuccess)
        {
            AppState.CurrentUser = userResult.Value;
        }

        Navigation.NavigateTo("/", forceLoad: false);

    }

    private bool NotifyIfError(Result result)
    {
        if (result.IsFailure)
        {
            var error = result.Error!;

            if (error.Type == ErrorType.Validation)
            {
                errorMessages = error.Details;
            }
            else
            {
                errorMessages = [error.Description];
            }
            isLoading = false;
            StateHasChanged();
            return true;
        }
        isLoading = false;
        StateHasChanged();
        return false;
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