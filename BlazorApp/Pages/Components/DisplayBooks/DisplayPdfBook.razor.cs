using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Objects.Entities.Books.PdfBook;
using Objects.Entities.Translator;
using Services;
using System.Data.Common;

namespace BlazorApp.Pages.Components.DisplayBooks
{
    public partial class DisplayPdfBook : ComponentBase, IDisplayBook
    {
        [Inject] IJSRuntime JS { get; set; } = null!;
        [Inject] BookOperationsService BookOperationsService { get; set; } = null!;
        [Inject] TranslatorService TranslatorService { get; set; } = null!;
        [Parameter] public string BookId { get; set; } = null!;

        public int ActualPageNumber { get; set; } = 0;
        public int PagesCount { get; set; } = 0;

        private PdfBook? _book;
        private bool _isLoading;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            _book = await BookOperationsService.GetBookText(Guid.Parse(BookId));
            await JS.InvokeVoidAsync("initializeBookContainer", _book.Text, DotNetObjectReference.Create(TranslatorService));

            _isLoading = false;
            await base.OnInitializedAsync();
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
