using AngleSharp.Html.Parser;
using BlazorApp;
using Blazored.LocalStorage;
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
builder.Services.AddScoped<EpubConverter>();
builder.Services.AddSingleton<ProgressService>();
builder.Services.AddScoped<BookCoverService>();

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
