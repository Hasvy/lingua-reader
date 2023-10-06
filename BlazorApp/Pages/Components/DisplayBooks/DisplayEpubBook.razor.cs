using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Objects.Entities.Books.EpubBook;
using Services;

namespace BlazorApp.Pages.Components.DisplayBooks
{
    public partial class DisplayEpubBook : ComponentBase, IDisplayBook
    {
        [Inject] IJSRuntime JS { get; set; } = null!;
        [Inject] BookOperationsService BookOperationsService { get; set; } = null!;
        [Parameter] public string BookId { get; set; } = null!;

        public int ActualPageNumber { get; set; } = 1;
        public int PagesCount { get; set; }

        private IList<BookSection> Sections = new List<BookSection>();
        private int actualSectionPagesCount = 0;
        private int currentSectionNumber;
        private bool _isLoading;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            Sections = await BookOperationsService.GetBookSections(Guid.Parse(BookId));

            await JS.InvokeVoidAsync("initializeBookContainer");
            await JS.InvokeVoidAsync("setClone");

            foreach (var item in Sections)                  //TODO what if section will be less then one page? Maybe it on page will be only content of the section.
            {
                item.PagesCount = await JS.InvokeAsync<int>("separateHtmlOnPages", item.Text);
                item.FirstPage = PagesCount + 1;
                item.LastPage = PagesCount + item.PagesCount;
                PagesCount += item.PagesCount;
                if (item.PagesCount == 0 || item.LastPage == 0)
                {
                    throw new Exception();
                }
            }
            await JS.InvokeVoidAsync("removeClone");

            currentSectionNumber = 0;
            actualSectionPagesCount = await JS.InvokeAsync<int>("divideAndSetHtml", Sections.First().Text);

            await JS.InvokeVoidAsync("showContent");
            _isLoading = false;
            await base.OnInitializedAsync();
            //stopwatch.Stop();
            //Console.WriteLine("Time: " + stopwatch.ElapsedMilliseconds + " msec");
        }

        public async void ChangePage(int? pageNumber)
        {
            foreach (var section in Sections)
            {
                if (section.FirstPage <= pageNumber && pageNumber <= section.LastPage)
                {
                    ActualPageNumber = (int)pageNumber;
                    currentSectionNumber = section.OrderNumber;
                    actualSectionPagesCount = await JS.InvokeAsync<int>("divideAndSetHtml", section.Text);
                    await JS.InvokeVoidAsync("setActualPage", ActualPageNumber - section.FirstPage);
                }
            }
        }

        public async void NextPage()
        {
            if (ActualPageNumber != PagesCount)
            {
                ActualPageNumber++;
                if (ActualPageNumber > Sections[currentSectionNumber].LastPage)
                {
                    NextSection();
                }
                else
                {
                    await JS.InvokeVoidAsync("nextPage");
                }
            }
        }

        public async void PreviousPage()
        {
            if (ActualPageNumber > 1)
            {
                ActualPageNumber--;
                if (ActualPageNumber < Sections[currentSectionNumber].FirstPage)
                {
                    PreviousSection();
                    await JS.InvokeVoidAsync("setScrollToLastPage", Sections[currentSectionNumber].PagesCount);
                }
                else
                {
                    await JS.InvokeVoidAsync("previousPage");
                }
            }
        }

        private async void NextSection()
        {
            if (currentSectionNumber < Sections.Count())
            {
                currentSectionNumber++;
                await JS.InvokeAsync<int>("divideAndSetHtml", Sections[currentSectionNumber].Text);
            }
        }

        private async void PreviousSection()
        {
            if (currentSectionNumber > 0)
            {
                currentSectionNumber--;
                await JS.InvokeAsync<int>("divideAndSetHtml", Sections[currentSectionNumber].Text);
            }
        }
    }
}
