using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Objects.Entities;
using Objects.Entities.Translator;
using Services;
using System.Net.Http;
using System.Net.Http.Json;

namespace BlazorApp.Pages.Components.Translator
{
    public partial class Translator : ComponentBase
    {
        [Inject] TranslatorService TranslatorService { get; set; } = null!;
        string word = "hello";
        string initialLang = "en";
        string targetLang = "cz";

        private TranslatorWordResponse? responseContent;
        //private static HttpClient _httpClient;
        //private TranslatorService? TranslatorService;
        private string error = string.Empty;

        //public Translator()
        //{
        //    _httpClient = new HttpClient();
        //    _httpClient.BaseAddress = new Uri("http://localhost:5284");
        //    _httpClient.Timeout = TimeSpan.FromSeconds(30);
        //    TranslatorService = new TranslatorService(_httpClient);
        //}

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        [JSInvokable]
        public static async Task GetWordFromJS(string word)
        {
            if (word.Length < 10)       //Temporary
            {
                Translator component = new Translator();        //TODO Change this
                await component.GetTranslation(word);
            }
        }

        public async Task GetTranslation(string word)
        {
            responseContent = await TranslatorService.GetWordTranslation(word);
        }
    }
}
