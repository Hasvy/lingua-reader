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

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5230/") });
//Radzen
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
//Html
builder.Services.AddScoped<HtmlParser>();
//My services
builder.Services.AddScoped<EpubConverter>();
builder.Services.AddScoped<BookOperationsService>();
builder.Services.AddScoped<AddBookService>();

builder.Services.AddSingleton<ProgressService>();

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
