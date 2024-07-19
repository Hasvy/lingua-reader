using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Services;

namespace BlazorApp.Pages.MainPages
{
    public partial class TryApp : ComponentBase
    {
        [Inject] IJSRuntime JS { get; set; } = null!;
        [Inject] HttpClient HttpClient { get; set; } = null!;
        [Inject] FileService FileService { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        private string BookLanguage { get; set; } = "en";
        private int CurrentPageNumber { get; set; } = 1;

        public int PagesCount { get; set; } = 0;
        private string exampleText = string.Empty;

        public async Task AfterReaderReady()
        {
            await JS.InvokeVoidAsync("onInitialized", BookLanguage);
            var response = await HttpClient.GetAsync($"{NavigationManager.BaseUri}/example/example.html");
            response.EnsureSuccessStatusCode();
            exampleText = await response.Content.ReadAsStringAsync();
            PagesCount = await JS.InvokeAsync<int>("embedHtmlOnPage", exampleText);
        }

        public async void JumpToPage(int? pageNumber)
        {
            if (1 <= pageNumber && pageNumber <= PagesCount)
            {
                CurrentPageNumber = (int)pageNumber;
                await JS.InvokeVoidAsync("jumpToPage", CurrentPageNumber - 1);
            }
        }

        public async void NextPage()
        {
            if (CurrentPageNumber != PagesCount)
            {
                CurrentPageNumber += 1;
                await JS.InvokeVoidAsync("nextPage", CurrentPageNumber - 1);
            }
        }

        public async void PreviousPage()
        {
            if (CurrentPageNumber > 1)
            {
                CurrentPageNumber -= 1;
                await JS.InvokeVoidAsync("previousPage", CurrentPageNumber - 1);
            }
        }
    }
}
