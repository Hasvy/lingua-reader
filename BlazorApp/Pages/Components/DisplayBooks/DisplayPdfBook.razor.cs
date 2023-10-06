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

        public int ActualPageNumber { get; set; }
        public int PagesCount { get; set; }

        private PdfBook? _book;
        private bool _isLoading;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            _book = await BookOperationsService.GetBookText(Guid.Parse(BookId));

            _isLoading = false;
            await base.OnInitializedAsync();
        }

        public void NextPage()
        {

        }

        public void PreviousPage()
        {
            throw new NotImplementedException();
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
