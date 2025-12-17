using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Tracker.Services;
using Tracker.Services.Abstraction;
using Tracker.Services.ApiClients;
using Tracker.WebApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);


builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<IAuthStorage, AuthStorage>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthHeaderHandler>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
builder.Services.AddApiClients();

// Add MudBlazor services
builder.Services.AddMudServices();

await builder.Build().RunAsync();
