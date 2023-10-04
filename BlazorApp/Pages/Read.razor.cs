using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Objects.Entities;
using Services;
using System.Reflection.Metadata;

namespace BlazorApp.Pages
{
    public partial class Read : ComponentBase
    {
        //[Inject] HtmlParser htmlParser { get; set; } = null!;
        //[Inject] HtmlParserService htmlParserService { get; set; } = null!;
        //[Inject] ILocalStorageService localStorageService { get; set; } = null!;
        [Inject] IJSRuntime JS { get; set; } = null!;
        [Inject] BookOperationsService BookOperationsService { get; set; } = null!;

        [Parameter]
        public string BookId { get; set; } = null!;
        private IList<BookSection> Sections { get; set; } = new List<BookSection>();
        private int _actualPageNumber = 1;
        private int pagesCount = 0;
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
                item.FirstPage = pagesCount + 1;
                item.LastPage = pagesCount + item.PagesCount;
                pagesCount += item.PagesCount;
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

        private async void ChangePage(int? pageNumber)
        {
            foreach (var section in Sections)
            {
                if (section.FirstPage <= pageNumber && pageNumber <= section.LastPage)
                {
                    _actualPageNumber = (int)pageNumber;
                    currentSectionNumber = section.OrderNumber;
                    actualSectionPagesCount = await JS.InvokeAsync<int>("divideAndSetHtml", section.Text);
                    await JS.InvokeVoidAsync("setActualPage", _actualPageNumber - section.FirstPage);
                }
            }
        }

        private async void NextPage()
        {
            if (_actualPageNumber != pagesCount)
            {
                _actualPageNumber++;
                if (_actualPageNumber > Sections[currentSectionNumber].LastPage)
                {
                    NextSection();
                }
                else
                {
                    await JS.InvokeVoidAsync("nextPage");
                }
            }
        }

        private async void PreviousPage()
        {
            if (_actualPageNumber > 1)
            {
                _actualPageNumber--;
                if (_actualPageNumber < Sections[currentSectionNumber].FirstPage)
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
