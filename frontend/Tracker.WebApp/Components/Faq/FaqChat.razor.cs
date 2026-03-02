using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Tracker.Services.Abstraction;
using Tracker.WebApp.States;

namespace Tracker.WebApp.Components.Faq;

public partial class FaqChat
{
    private string _input = "";
    private bool _isLoading = false;
    private ElementReference _scrollAnchor;

    [Inject] IFaqService FaqService { get; set; } = null!;
    [Inject] AppState AppState { get; set; } = null!;
    [Inject] IJSRuntime JS { get; set; }

    private List<ChatMessage> _messages = new()
    {
        new("Hi! What do you want to know about", false)
    };


    private async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(_input) || _isLoading)
        {
            return;
        }

        var question = _input.Trim();
        _input = "";
        _messages.Add(new(question, true));
        StateHasChanged();
        await ScrollToBottom();
        _isLoading = true;
        await ScrollToBottom();

        var result = await FaqService.AskAsync(question);
        if (result.IsSuccess)
        {
            _messages.Add(new(result.Value, false));
        }
        else
        {
            _messages.Add(new("Sorry, something went wrong. Please try again.", false));
        }
        _isLoading = false;
        StateHasChanged();
        await ScrollToBottom();
    }

    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        Console.WriteLine("Key event {0} with key {1}", e, e.Key);
        if (e.Key == "Enter" && !e.ShiftKey)
        {
            await SendMessage();
        }
    }

    private async Task ScrollToBottom()
    {
        await JS.InvokeVoidAsync("BlazorScrollToBottom", _scrollAnchor);
    }
}