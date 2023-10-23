using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Objects.Entities.Books.PdfBook;
using Services;

namespace BlazorApp.Pages.Components.DisplayBooks
{
    public partial class DisplayPdfBook : ComponentBase, IDisplayBook
    {
        [Inject] IJSRuntime JS { get; set; } = null!;
        [Inject] BookOperationsService BookOperationsService { get; set; } = null!;
        [Parameter] public string BookId { get; set; } = null!;

        public int ActualPageNumber { get; set; } = 0;
        public int PagesCount { get; set; } = 0;

        private PdfBook? _book;
        private bool _isLoading;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _isLoading = true;
            _book = await BookOperationsService.GetBookText(Guid.Parse(BookId));

            await JS.InvokeVoidAsync("initializeBookContainer", _book.Text);
            //await JS.InvokeVoidAsync("addText", _book.Text);
            //await JS.InvokeVoidAsync("divideHtmlOnPages");
            //await JS.InvokeVoidAsync("addEventListenerForTextClicked");

            //await JS.InvokeVoidAsync("getText", _book.Text);

            _isLoading = false;
        }

        public async void NextPage()
        {
            await JS.InvokeVoidAsync("nextPage");
        }

        public async void PreviousPage()
        {
            await JS.InvokeVoidAsync("previousPage");
        }

        public void ChangePage(int? pageNumber)
        {
            throw new NotImplementedException();
        }

        //private int GetIndexOfActualPage()
        //{
        //    return Text.IndexOf(_actualPage);
        //}

        //private async void NextPage()
        //{
        //    await SetActualPageText();
        //    if (ActualPageNumber != Text.Count)
        //    {
        //        _actualPage = Text[GetIndexOfActualPage() + 1];
        //        ActualPageNumber++;
        //    }
        //}

        //private void PreviousPage()
        //{
        //    if (ActualPageNumber != 1)
        //    {
        //        _actualPage = Text[GetIndexOfActualPage() - 1];
        //        ActualPageNumber--;
        //    }
        //}
    }
}
