using BlazorApp.Components.DisplayBooks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Objects.Entities.Books.PdfBook;
using Objects.Entities.Translator;
using Services;

namespace BlazorApp.Components.DisplayBooks
{
    public partial class DisplayPdfBook : ComponentBase, IDisplayBook
    {
        [Inject] IJSRuntime JS { get; set; } = null!;
        [Inject] BookOperationsService BookOperationsService { get; set; } = null!;
        [Parameter] public string BookId { get; set; } = null!;
        [Parameter] public string BookLanguage {  get; set; } = null!;
        [Parameter] public string UserMainLang {  get; set; } = null!;

        public int CurrentPageNumber { get; set; } = 0;
        public int PagesCount { get; set; } = 0;

        private PdfBook? _book;
        private bool _isLoading;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            _book = await BookOperationsService.GetBookText(Guid.Parse(BookId));
            await JS.InvokeVoidAsync("onInitialized", BookLanguage);
            PagesCount = await JS.InvokeAsync<int>("embedHtmlOnPage", _book.Text);
            CurrentPageNumber = 1;
            _isLoading = false;
            await base.OnInitializedAsync();
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
