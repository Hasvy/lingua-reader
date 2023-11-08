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
    public partial class TranslatorWindow : ComponentBase
    {
        //[Inject] TranslatorService TranslatorService { get; set; } = null!;
        [Parameter] public WordInfo WordInfo { get; set; }
        [Parameter] public bool Hidden { get; set; }
        //[Parameter] public float Height { get; set; }
        //[Parameter] public float Left { get; set; }
        //[Parameter] public float Top { get; set; }
        //public static event Action OnUpdate = null!;
        //private static string? wordToTranslate;
        [Parameter] public TranslatorWordResponse? ResponseContent { get; set; }
        private string error = string.Empty;

        protected override Task OnInitializedAsync()
        {
            
            return base.OnInitializedAsync();
        }

        //Try to understand this more, and try to optimize it
        [JSInvokable]           //Optimize it, maybe just get a word, and then update it with Invoke and // Works worse.
                                //Mb bcs translator service instance creates in DisplayBook, not in TranslatorWindow, so creates a service for every translation.
        
        //public static async Task GetWordFromJS(string word)
        //public static async Task GetWordFromJS(string word, float height, float width, float left, float top, DotNetObjectReference<TranslatorService> translatorServiceInstance)
        //{
        //    if (word.Length < 10)       //Temporary
        //    {
        //        var translatorService = translatorServiceInstance.Value;
        //        wordToTranslate = word;
        //        Height = height;
        //        Left = left;
        //        Top = top;
        //        //responseContent = await translatorService.GetWordTranslation(word);
        //        OnUpdate?.Invoke();
        //    }
        //}

        //[JSInvokable]
        //public static void GetTranslatorPositionFromJS()
        //{
        //    Console.WriteLine($"{width}, {left}, {top}");
        //}

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
