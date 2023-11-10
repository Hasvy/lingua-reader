using Microsoft.AspNetCore.Components;
using Objects.Entities.Translator;
using System.Diagnostics;

namespace BlazorApp.Pages.Components.Translator
{
    public partial class TranslatorWindow : ComponentBase
    {
        [Parameter] public WordInfo WordInfo { get; set; }
        [Parameter] public bool Hidden { get; set; }
        //[Parameter] public EventCallback ProcessResponse { get; set; }
        [Parameter] public TranslatorWordResponse? ResponseContent { get; set; }

        private string error = string.Empty;
        private WordTranslation? mainTranslation;
        //private List<Dictionary<string, string[]>> tagWords = new List<Dictionary<string, string[]>>();
        private Dictionary<string, string> dict = new Dictionary<string, string>();

        protected override async Task OnInitializedAsync()
        {
            ProcessResponse();
            await base.OnInitializedAsync();
        }

        public void ProcessResponse()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (ResponseContent is not null)
            {
                mainTranslation = ResponseContent.Translations.FirstOrDefault();

                if (mainTranslation != null)
                {
                    dict = ResponseContent.Translations
                        .GroupBy(t => t.PosTag)
                        .ToDictionary(
                            group => group.Key,
                            group => string.Join(" ", group.Select(t => t.DisplayTarget))
                        );
                }
            }
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            
            await base.OnAfterRenderAsync(firstRender);
        }

        //private void Speak(string word)
        //{
        //    speechSynthesizer.SpeakAsync(word);
        //}
    }
}
