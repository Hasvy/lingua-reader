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
builder.Services.AddScoped<EpubConverter>();        //TODO check scoped and singleton services
builder.Services.AddScoped<BookOperationsService>();
builder.Services.AddScoped<FilesOperationsService>();
builder.Services.AddScoped<AddBookService>();
builder.Services.AddScoped<HtmlParserService>();
builder.Services.AddScoped<TranslatorService>();

builder.Services.AddSingleton<ProgressService>();
builder.Services.AddSingleton<TranslatorServiceAccessor>();

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
