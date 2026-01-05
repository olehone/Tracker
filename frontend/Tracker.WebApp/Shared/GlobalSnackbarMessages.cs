using MudBlazor;
using Tracker.Domain.Results;

namespace Tracker.WebApp.Shared;

public class GlobalSnackbarMessages(ISnackbar SnackbarService) : IErrorNotifier
{
    public bool NotifyIfError(Result result)
    {
        if (result.IsSuccess)
        {
            return false;
        }
        var error = result.Error!;
        NotifyActionError(error);

        return true;
    }

    // Validation errors is handled by forms
    public void NotifyActionError(Error error)
    {
        if (error.Type is ErrorType.Validation|| error.Type is ErrorType.Network)
        {
            return;
        }

        SnackbarService.Add(error.Description!, Severity.Error);
    }

    // In case of future implementation of retry/offline
    public void NotifyNetworkError(Error error)
    {
        var networkConfig = (SnackbarOptions config) =>
        {
            config.DuplicatesBehavior = SnackbarDuplicatesBehavior.Prevent;
        };

        if (error.Type is ErrorType.Network)
        {
            SnackbarService.Add(error.Description!, Severity.Warning, networkConfig, key: "offline");
        }
    }
}
