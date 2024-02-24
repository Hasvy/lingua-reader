using BlazorApp.Components.DisplayBooks;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Objects.Entities.Books.EpubBook;
using Services;
using System.Diagnostics;

namespace BlazorApp.Components.DisplayBooks
{
    public partial class DisplayEpubBook : ComponentBase, IDisplayBook
    {
        [Inject] IJSRuntime JS { get; set; } = null!;
        [Inject] BookOperationsService BookOperationsService { get; set; } = null!;
        [Parameter] public string BookId { get; set; } = null!;
        [Parameter] public string BookLanguage { get; set; } = null!;
        [Parameter] public string UserMainLang { get; set; } = null!;

        public int CurrentPageNumber { get; set; }
        public int PagesCount { get; set; }

        private IList<BookSection> Sections = new List<BookSection>();
        private int actualSectionPagesCount = 0;
        private int currentSectionNumber;
        private bool _isLoading;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            //Stopwatch stopwatch = Stopwatch.StartNew();
            Sections = await BookOperationsService.GetBookSections(Guid.Parse(BookId));

            await JS.InvokeVoidAsync("onInitialized", BookLanguage);

            foreach (var section in Sections)
            {
                section.PagesCount = await JS.InvokeAsync<int>("embedHtmlOnPage", section.Text);
                section.FirstPage = PagesCount + 1;
                section.LastPage = PagesCount + section.PagesCount;
                PagesCount += section.PagesCount;
                if (section.PagesCount == 0 || section.LastPage == 0)
                {
                    throw new Exception();
                }
                //TODO Fix Epub display styles from inner html tag
            }

            CurrentPageNumber = 1;
            currentSectionNumber = 0;
            actualSectionPagesCount = await JS.InvokeAsync<int>("embedHtmlOnPage", Sections[0].Text);

            await JS.InvokeVoidAsync("showContent");
            _isLoading = false;
            await base.OnInitializedAsync();
            //stopwatch.Stop();
            //Console.WriteLine("Time: " + stopwatch.ElapsedMilliseconds + " msec");
        }

        public async void JumpToPage(int? pageNumber)
        {
            foreach (var section in Sections)
            {
                if (section.FirstPage <= pageNumber && pageNumber <= section.LastPage)
                {
                    CurrentPageNumber = (int)pageNumber;
                    currentSectionNumber = section.OrderNumber;
                    actualSectionPagesCount = await JS.InvokeAsync<int>("embedHtmlOnPage", section.Text);
                    await JS.InvokeVoidAsync("jumpToPage", CurrentPageNumber - section.FirstPage);
                }
            }
        }

        public async void NextPage()
        {
            if (CurrentPageNumber != PagesCount)
            {
                CurrentPageNumber++;
                if (CurrentPageNumber > Sections[currentSectionNumber].LastPage)
                {
                    NextSection();
                }
                else
                {
                    await JS.InvokeVoidAsync("nextPage", CurrentPageNumber - Sections[currentSectionNumber].FirstPage);
                }
            }
        }

        public async void PreviousPage()
        {
            if (CurrentPageNumber > 1)
            {
                CurrentPageNumber--;
                if (CurrentPageNumber < Sections[currentSectionNumber].FirstPage)
                {
                    await PreviousSection();
                }
                else
                {
                    await JS.InvokeVoidAsync("previousPage", CurrentPageNumber - Sections[currentSectionNumber].FirstPage);
                }
            }
        }

        private async void NextSection()
        {
            if (currentSectionNumber < Sections.Count())
            {
                currentSectionNumber++;
                await JS.InvokeAsync<int>("embedHtmlOnPage", Sections[currentSectionNumber].Text);
            }
        }

        private async Task PreviousSection()
        {
            if (currentSectionNumber > 0)
            {
                currentSectionNumber--;
                await JS.InvokeAsync<int>("embedHtmlOnPage", Sections[currentSectionNumber].Text);
                await JS.InvokeVoidAsync("setScrollToLastPage", Sections[currentSectionNumber].PagesCount - 1);
            }
        }
    }
}
