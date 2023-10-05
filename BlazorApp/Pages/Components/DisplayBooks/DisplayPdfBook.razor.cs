using Microsoft.AspNetCore.Components;

namespace BlazorApp.Pages.Components.DisplayBooks
{
    public partial class DisplayPdfBook : ComponentBase
    {
        [Parameter] public string BookId { get; set; } = null!;

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        //private int GetIndexOfActualPage()
        //{
        //    return Text.IndexOf(_actualPage);
        //}

        //private async void NextPage()
        //{
        //    await SetActualPageText();
        //    if (_actualPageNumber != Text.Count)
        //    {
        //        _actualPage = Text[GetIndexOfActualPage() + 1];
        //        _actualPageNumber++;
        //    }
        //}

        //private void PreviousPage()
        //{
        //    if (_actualPageNumber != 1)
        //    {
        //        _actualPage = Text[GetIndexOfActualPage() - 1];
        //        _actualPageNumber--;
        //    }
        //}
    }
}
