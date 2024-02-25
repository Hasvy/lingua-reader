using AngleSharp.Io;
using BlazorApp.Components.Translator;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Objects.Entities.Translator;
using Org.BouncyCastle.Crypto.Parameters;
using Services;

namespace BlazorApp.Components.DisplayBooks
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

        private int? insertedPageNumber;
        private ElementReference host;
        private bool visible = false;
        private bool isBusy = false;
        private bool isLoading = false;
        private WordInfo? wordInfo = new WordInfo();
        private WordWithTranslations? translatorWordResponse;
        private string pressedKey = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await TranslatorService.SetLanguages(BookLanguage, UserMainLang);
            await base.OnInitializedAsync();
        }

        private async Task GetSelectedWord()        //TODO reorganize the method and rename
        {
            visible = false;
            wordInfo = await JS.InvokeAsync<WordInfo>("getSelectedWord", host);
            if (!string.IsNullOrWhiteSpace(wordInfo.Word) && wordInfo.Word.Length < 30)
            {
                visible = true;          //ShowTranslatorWindow, unhide
                isLoading = true;
                StateHasChanged();
                //await Task.Yield();
                await Task.Delay(1);
                translatorWordResponse = await TranslatorService.GetWordTranslation(wordInfo.Word);
            }
            //StateHasChanged();
            //translatorWindow.ProcessResponse();
            isLoading = false;
        }

        private async Task SpeakWord()
        {
            if (wordInfo is not null)
            {
                isBusy = true;
                await Task.Delay(1);            //Isntead of StateHasChanged(); which does not work because of a bug
                await JS.InvokeVoidAsync("speakWord", wordInfo.Word, BookLanguage);
                isBusy = false;
            }
        }

        private async void HandleKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "ArrowRight" || e.Key == " ")
            {
                await NextPage();
            }
            if (e.Key == "ArrowLeft")
            {
                await PreviousPage();
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

        private async Task JumpToPage(int? insertedPageNumber)
        {
            await JumpToPageCallback.InvokeAsync(insertedPageNumber);
        }
    }
}
