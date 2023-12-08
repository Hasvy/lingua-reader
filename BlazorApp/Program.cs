using AngleSharp.Html.Parser;
using BlazorApp;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//Authentication
builder.Services.AddHttpClient("LinguaReader.ServerAPI",
    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("LinguaReader.ServerAPI"));

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

builder.Services.AddSingleton<ProgressService>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddApiAuthorization();

await builder.Build().RunAsync();
