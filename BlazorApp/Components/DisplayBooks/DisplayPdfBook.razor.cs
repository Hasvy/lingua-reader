using BlazorApp.Components.DisplayBooks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Objects.Constants;
using Objects.Entities.Books;
using Objects.Entities.Books.PdfBook;
using Objects.Entities.Books.TxtBook;
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
        [Parameter] public string BookFormat {  get; set; } = null!;

        public int CurrentPageNumber { get; set; } = 0;
        public int PagesCount { get; set; } = 0;

        //private PdfBook? _book;
        private bool _isLoading;

        public async Task AfterReaderReady()
        {
            _isLoading = true;
            await JS.InvokeVoidAsync("onInitialized", BookLanguage);
            CurrentPageNumber = 1;

            if (BookFormat is ConstBookFormats.pdf)
            {
                var pdfBook = await BookOperationsService.GetPdfBookText(Guid.Parse(BookId));
                PagesCount = await JS.InvokeAsync<int>("embedHtmlOnPage", pdfBook.Text);
            }

            if (BookFormat is ConstBookFormats.txt)
            {
                var txtBook = await BookOperationsService.GetTxtBookText(Guid.Parse(BookId));
                PagesCount = await JS.InvokeAsync<int>("embedHtmlOnPage", txtBook.Text);
            }
            _isLoading = false;
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
