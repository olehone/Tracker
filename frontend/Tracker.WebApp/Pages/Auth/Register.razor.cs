using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Requests;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction;
using Tracker.WebApp.Models;
using Tracker.WebApp.Shared;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages.Auth;

public partial class Register
{
    private readonly RegisterUserModel _registerModel = new();
    private IReadOnlyList<string>? _errorMessages;
    private MudForm _form;
    private bool _isLoading;
    private bool _isSuccess;
    private string? _secondPassword;

    [CascadingParameter]
    private AppState? AppState { get; set; }

    [Inject] private IAuthService Auth { get; set; } = null!;
    [Inject] private IUserService UserService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private async Task HandleRegister()
    {
        await _form!.Validate();

        if (!_form.IsValid)
        {
            return;
        }

        if (_registerModel.Password != _secondPassword)
        {
            _errorMessages = ["Passwords are not the same"];
            return;
        }

        if (UiHelper.IsEmailInvalid(_registerModel.Email))
        {
            _errorMessages = ["Wrong email format"];
            return;
        }

        _isLoading = true;

        var result = await Auth.RegisterAsync(ToRequest(_registerModel));
        _isLoading = false;
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

            return true;
        }

        _isLoading = false;
        StateHasChanged();
        return false;
    }

    private static RegisterUserRequest ToRequest(RegisterUserModel model)
    {
        return new RegisterUserRequest
        {
            Email = model.Email,
            Password = model.Password,
            Username = model.Username,
            FirstName = model.FirstName,
            LastName = model.LastName
        };
    }
}