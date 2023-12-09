using AngleSharp.Html.Parser;
using BlazorApp;
using Blazored.LocalStorage;
using Services.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7049/") });
//Radzen
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
//Html
builder.Services.AddScoped<HtmlParser>();
//My services
builder.Services.AddScoped<BookOperationsService>();        //TODO check scoped and singleton services
builder.Services.AddScoped<AddBookService>();
builder.Services.AddScoped<HtmlParserService>();
builder.Services.AddScoped<TranslatorService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
builder.Services.AddScoped<AuthStateProvider>();

builder.Services.AddSingleton<ProgressService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

await builder.Build().RunAsync();
