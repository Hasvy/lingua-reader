using Microsoft.AspNetCore.Components;
using Objects.Entities;
using Objects.Entities.Translator;
using Services;
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
        private string error = string.Empty;

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        public async void GetTranslation()
        {
            responseContent = await TranslatorService.GetWordTranslation();
        }
    }
}
