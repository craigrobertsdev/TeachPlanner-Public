using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Icons.FontAwesome;
using Blazorise.RichTextEdit;
using Blazorise.Tailwind;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using TeachPlanner.BlazorClient;
using TeachPlanner.BlazorClient.DependencyInjection;
using TeachPlanner.BlazorClient.Handlers;
using TeachPlanner.BlazorClient.Services;
using TeachPlanner.BlazorClient.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthenticationServices();
builder.Services.AddTransient<AuthenticationHandler>();

builder.Services.AddHttpClient("ServerApi")
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["ServerUrl"] ?? ""))
    .AddHttpMessageHandler<AuthenticationHandler>();
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
builder.Services.AddSingleton<ApplicationState>();
builder.Services.AddBlazoredLocalStorageAsSingleton();
builder.Services.AddMudServices();
builder.Services
    .AddBlazorise()
    .AddTailwindProviders()
    .AddFontAwesomeIcons()
    .AddBlazoriseRichTextEdit();

var host = builder.Build();

// Load initial application state before rendering components
await host.Services.GetRequiredService<ApplicationState>().LoadState();

await host.RunAsync();