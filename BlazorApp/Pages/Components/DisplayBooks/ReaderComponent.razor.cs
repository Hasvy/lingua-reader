using BlazorApp.Pages.Components.Translator;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Objects.Entities.Translator;
using Services;

namespace BlazorApp.Pages.Components.DisplayBooks
{
    public partial class ReaderComponent : ComponentBase
    {
        [Inject] IJSRuntime JS { get; set; } = null!;
        [Inject] TranslatorService TranslatorService { get; set; } = null!;
        [Parameter] public int PagesCount { get; set; }
        [Parameter] public int CurrentPageNumber { get; set; }
        [Parameter] public EventCallback PreviousPageCallback { get; set; }
        [Parameter] public EventCallback NextPageCallback { get; set; }
        [Parameter] public EventCallback<int?> JumpToPageCallback { get; set; }
        [Parameter] public string BookLanguage { get; set; } = null!;
        [Parameter] public string UserMainLang { get; set; } = null!;

        public WordInfo? WordInfo { get; set; } = new WordInfo();

        private int? insertedPageNumber;
        private ElementReference host;
        private bool isTranslatorHidden;
        private TranslatorWordResponse? translatorWordResponse;
        private TranslatorWindow translatorWindow;      //???

        protected override async Task OnInitializedAsync()
        {
            await TranslatorService.SetLanguages(BookLanguage, UserMainLang);
            await base.OnInitializedAsync();
        }

        private void HideTranslatorWindow()
        {
            isTranslatorHidden = true;
        }

        private async Task GetSelectedWord()
        {
            WordInfo wordInfo = await JS.InvokeAsync<WordInfo>("getSelectedWord", host);
            if (!string.IsNullOrWhiteSpace(wordInfo.Word) && wordInfo.Word.Length < 9)
            {
                WordInfo = wordInfo;
                isTranslatorHidden = false;
                translatorWordResponse = await TranslatorService.GetWordTranslation(wordInfo.Word);
                StateHasChanged();
                translatorWindow.ProcessResponse();
            }
        }

        private async Task NextPage()
        {
            await NextPageCallback.InvokeAsync();
        }

        private async Task PreviousPage()
        {
            await PreviousPageCallback.InvokeAsync();
        }

        private async Task JumpToPage()
        {
            await JumpToPageCallback.InvokeAsync(insertedPageNumber);
        }
    }
}
