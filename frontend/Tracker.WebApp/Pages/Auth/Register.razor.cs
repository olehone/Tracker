using Microsoft.AspNetCore.Components;
using MudBlazor;
using Tracker.Domain.Requests;
using Tracker.Domain.Results;
using Tracker.Services.Abstraction.Entities;
using Tracker.WebApp.Models;
using Tracker.WebApp.Shared;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Pages.Auth;

public partial class Register
{
    [CascadingParameter] private AppState? AppState { get; set; }

    [Inject] private IAuthService Auth { get; set; } = null!;
    [Inject] private IUserService UserService { get; set; } = null!;
    [Inject] private NavigationManager Navigation { get; set; } = null!;

    private RegisterUserModel registerModel = new();
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
        if (UiHelper.IsEmailInvalid(registerModel.Email))
        {
            errorMessages = ["Wrong email format"];
            return;
        }
        isLoading = true;

        var result = await Auth.RegisterAsync(ToRequest(registerModel));
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
            return true;
        }
        isLoading = false;
        StateHasChanged();
        return false;
    }

    private static RegisterUserRequest ToRequest(RegisterUserModel model)
    {
        return new RegisterUserRequest()
        {
            Email = model.Email,
            Password = model.Password,
            Username = model.Username,
            FirstName = model.FirstName,
            LastName = model.LastName
        };
    }
}