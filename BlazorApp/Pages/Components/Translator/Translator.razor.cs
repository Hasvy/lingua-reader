using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using Objects.Entities;
using Objects.Entities.Translator;
using Services;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace BlazorApp.Pages.Components.Translator
{
    public partial class Translator : ComponentBase
    {
        //[Inject] TranslatorService TranslatorService { get; set; } = null!;
        public static event Action OnUpdate = null!;
        private static string? wordToTranslate;
        private static TranslatorWordResponse? responseContent;
        private string error = string.Empty;

        protected override Task OnInitializedAsync()
        {
            OnUpdate += HandleUpdate;
            return base.OnInitializedAsync();
        }

        //Try to understand this more, and try to optimize it
        [JSInvokable]           //Optimize it, maybe just get a word, and then update it with Invoke and // Works worse.
                                //Mb bcs translator service instance creates in DisplayBook, not in Translator, so creates a service for every translation.
        
        //public static async Task GetWordFromJS(string word)
        public static async Task GetWordFromJS(string word, DotNetObjectReference<TranslatorService> translatorServiceInstance)
        {
            if (word.Length < 10)       //Temporary
            {
                var translatorService = translatorServiceInstance.Value;
                wordToTranslate = word;
                responseContent = await translatorService.GetWordTranslation(word);
                OnUpdate?.Invoke();
            }
        }

        //private void Speak(string word)
        //{
        //    speechSynthesizer.SpeakAsync(word);
        //}

        private void HandleUpdate()
        {
            StateHasChanged();
        }
    }
}
