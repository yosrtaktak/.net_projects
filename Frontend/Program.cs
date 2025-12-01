using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Frontend;
using Frontend.Services;
using Blazored.LocalStorage;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HttpClient with backend API URL
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5000/") });

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Add MudBlazor Services
builder.Services.AddMudServices();

// Add custom services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<AuthStateProvider>();

var host = builder.Build();

// Initialize authentication state
var authStateProvider = host.Services.GetRequiredService<AuthStateProvider>();
await authStateProvider.InitializeAuthenticationStateAsync();

await host.RunAsync();
