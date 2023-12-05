using Microsoft.AspNetCore.Components;
using Objects.Entities.Translator;
using System.Diagnostics;
using BlazorApp.Pages.Components.DisplayBooks;
using Microsoft.JSInterop;

namespace BlazorApp.Pages.Components.Translator
{
    public partial class TranslatorWindow : ComponentBase
    {
        [Inject] IJSRuntime JS { get; set; } = null!;
        [Parameter] public WordInfo WordInfo { get; set; }
        [Parameter] public TranslatorWordResponse? ResponseContent { get; set; }
        [Parameter] public EventCallback SpeakWordCallback { get; set; }
        [Parameter] public bool Visible { get; set; } = false;
        [Parameter] public bool isBusy { get; set; } = false;
        [Parameter] public bool isLoading { get; set; } = false;

        //private bool isLoading = false;
        private ElementReference translatorWindow;
        private string error = string.Empty;
        private WordTranslation? mainTranslation;
        //private List<Dictionary<string, string[]>> tagWords = new List<Dictionary<string, string[]>>();
        private Dictionary<string, string> dict = new Dictionary<string, string>();

        protected override void OnParametersSet()
        {
            ProcessResponse();
            base.OnParametersSet();
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            SpecifyWindowPosition();
            return base.OnAfterRenderAsync(firstRender);
        }

        private void ProcessResponse()
        {
            //Stopwatch stopwatch = Stopwatch.StartNew();
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
                    //isLoading = false;
                }
            }
            //stopwatch.Stop();
            //Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        private async void SpecifyWindowPosition()
        {
            await JS.InvokeVoidAsync("setWindowSizeVars");
        }

        private class Size
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }

        protected async Task SpeakWord()
        {
            await SpeakWordCallback.InvokeAsync();
        }

        //private async Task SpeakWord()            //TODO set BookLanguage while initializing in JS and send speak word from here
        //{
        //    if (WordInfo is not null)
        //    {
        //        isBusy = true;
        //        await Task.Delay(1);            //Isntead of StateHasChanged(); which does not work because of a bug
        //        await JS.InvokeVoidAsync("speakWord", WordInfo.Word, BookLanguage);
        //        isBusy = false;
        //    }
        //}
    }
}
