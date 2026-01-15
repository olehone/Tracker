using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Tracker.Services;
using Tracker.WebApp;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Logging.AddFilter("Polly", LogLevel.None);
builder.Logging.AddFilter("Microsoft.Extensions.Http", LogLevel.Warning);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthServices();
builder.Services.AddApiAndServices();
builder.Services.AddMudBlazorServices();

await builder.Build().RunAsync();