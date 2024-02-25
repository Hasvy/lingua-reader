using Microsoft.AspNetCore.Components;
using Objects.Entities.Translator;
using Microsoft.JSInterop;
using Services;

namespace BlazorApp.Components.Translator
{
    public partial class TranslatorWindow : ComponentBase
    {
        [Inject] IJSRuntime JS { get; set; } = null!;
        [Inject] WordsService WordsService { get; set; } = null!;
        [Parameter] public WordInfo WordInfo { get; set; }
        [Parameter] public WordWithTranslations? WordWithTranslations { get; set; }
        [Parameter] public EventCallback SpeakWordCallback { get; set; }
        [Parameter] public bool Visible { get; set; } = false;
        [Parameter] public bool isSpeaking { get; set; } = false;
        [Parameter] public bool isLoading { get; set; } = false;

        private bool _isSaving = false;
        private bool _isDeleting = false;
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
            if (WordWithTranslations is not null)
            {
                mainTranslation = WordWithTranslations.Translations.FirstOrDefault();

                if (mainTranslation != null)
                {
                    dict = WordWithTranslations.Translations
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

        protected async Task SpeakWord()
        {
            await SpeakWordCallback.InvokeAsync();
        }

        //private async Task SpeakWord()            //TODO set BookLanguage while initializing in JS and send speak word from here
        //{
        //    if (WordInfo is not null)
        //    {
        //        isSpeaking = true;
        //        await Task.Delay(1);            //Isntead of StateHasChanged(); which does not work because of a bug
        //        await JS.InvokeVoidAsync("speakWord", WordInfo.Word, BookLanguage);
        //        isSpeaking = false;
        //    }
        //}

        private async Task AddWord()
        {
            _isSaving = true;
            bool result = await WordsService.SaveWord(WordWithTranslations);
            if (result is true)
                WordWithTranslations.IsWordSaved = true;
            _isSaving = false;
        }
        private async Task DeleteWord()
        {
            _isDeleting = true;
            bool result = await WordsService.DeleteWord(WordWithTranslations);
            if (result is true)
                WordWithTranslations.IsWordSaved = false;
            _isDeleting = false;
        }
    }
}
