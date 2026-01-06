using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Requests;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Models;
using Tracker.WebApp.Shared;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages.Auth;

public partial class Login
{
    private readonly LoginUserModel _loginModel = new();
    private IReadOnlyList<string>? _errorMessages = [];
    private MudForm _form;
    private bool _isLoading;
    private bool _isSuccess;

    [CascadingParameter]
    private AppState? AppState { get; set; }

    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private IUserService UserService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private async Task HandleLogin()
    {
        await _form!.Validate();

        if (!_form.IsValid)
        {
            return;
        }

        if (UiHelper.IsEmailInvalid(_loginModel.Email))
        {
            _errorMessages = ["Wrong email format"];
            return;
        }

        _isLoading = true;

        var result = await AuthService.LoginAsync(ToRequest(_loginModel));
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

        Navigation.NavigateTo("/", false);
    }

    private bool NotifyIfError(Result result)
    {
        if (result.IsFailure)
        {
            var error = result.Error!;

            if (error.Type == ErrorType.Validation)
            {
                _errorMessages = error.Details;
            }
            else
            {
                _errorMessages = [error.Description];
            }

            _isLoading = false;
            StateHasChanged();
            return true;
        }

        _isLoading = false;
        StateHasChanged();
        return false;
    }

    private static LoginUserRequest ToRequest(LoginUserModel model)
    {
        return new LoginUserRequest
        {
            Email = model.Email,
            Password = model.Password
        };
    }
}