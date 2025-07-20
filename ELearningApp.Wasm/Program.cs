using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using ELearningApp.Wasm;
using ELearningApp.Wasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configure HTTP client for API calls
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7001/") }); // Your API URL

// Add authentication
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();

// Add custom authentication state provider
builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();

// Add simplified services for now
builder.Services.AddScoped<ISimpleCourseService, SimpleCourseService>();
builder.Services.AddScoped<IUserService, UserService>();

await builder.Build().RunAsync();
