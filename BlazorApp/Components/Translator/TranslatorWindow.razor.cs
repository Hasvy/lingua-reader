using Microsoft.AspNetCore.Components;
using Objects.Entities.Translator;
using Microsoft.JSInterop;
using Services;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;

namespace BlazorApp.Components.Translator
{
    public partial class TranslatorWindow : ComponentBase
    {
        [Inject] IJSRuntime JS { get; set; } = null!;
        [Inject] WordsService WordsService { get; set; } = null!;
        [Inject] TranslatorService TranslatorService { get; set; } = null!;
        [Inject] AuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
        [Inject] NotificationService NotificationService { get; set; } = null!;
        [Parameter] public WordInfo WordInfo { get; set; }
        [Parameter] public WordWithTranslations? WordWithTranslations { get; set; }
        [Parameter] public EventCallback SpeakWordCallback { get; set; }
        [Parameter] public EventCallback OnWordChanged { get; set; }
        [Parameter] public bool Visible { get; set; } = false;
        [Parameter] public bool isSpeaking { get; set; } = false;
        [Parameter] public bool isLoading { get; set; } = false;

        private bool _isSaving = false;
        private bool _isDeleting = false;
        private bool _isUpdating = false;
        private ElementReference translatorWindow;
        private string error = string.Empty;
        private WordTranslation? mainTranslation;
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
            //else
            //{

            //}
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
            if (!(await CheckIfAuthorized()))
            {
                NotificationService.Notify(NotificationSeverity.Info, "You are not logged in", "Please log in to add words to your dictionary");
                return;
            }
            _isSaving = true;
            bool result = await WordsService.SaveWord(WordWithTranslations.Id);
            if (result is true)
                WordWithTranslations.IsWordSaved = true;
            _isSaving = false;
        }
        private async Task DeleteWord()
        {
            if (!(await CheckIfAuthorized()))
            {
                NotificationService.Notify(NotificationSeverity.Info, "You are not logged in", "Please log in to use this function");
                return;
            }
            _isDeleting = true;
            bool result = await WordsService.DeleteWord(WordWithTranslations.Id);
            if (result is true)
                WordWithTranslations.IsWordSaved = false;
            _isDeleting = false;
        }
        private async Task UpdateWord()
        {
            if (!(await CheckIfAuthorized()))
            {
                NotificationService.Notify(NotificationSeverity.Info, "You are not logged in", "Please log in to use this function");
                return;
            }
            _isUpdating = true;
            var result = await TranslatorService.UpdateWord(WordWithTranslations);
            if (result is not null)
            {
                WordWithTranslations = result;
                await OnWordChanged.InvokeAsync(WordWithTranslations);
            }
            _isUpdating = false;
        }

        private async Task<bool> CheckIfAuthorized()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            return (authState.User.Identity is null || !authState.User.Identity.IsAuthenticated) ? false : true;
        }
    }
}
