using Blazored.LocalStorage;
using BookingSystem.Client;
using BookingSystem.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();


builder.Services.AddAuthorizationCore(); // هسته اصلی Auth
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>(); // سرویس ما

await builder.Build().RunAsync();
