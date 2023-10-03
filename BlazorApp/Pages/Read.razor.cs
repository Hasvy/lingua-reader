using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Objects.Entities;
using Services;

namespace BlazorApp.Pages
{
    public partial class Read : ComponentBase
    {
        [Inject] HtmlParser htmlParser { get; set; } = null!;
        [Inject] HtmlParserService htmlParserService { get; set; } = null!;
        [Inject] IJSRuntime JS { get; set; } = null!;
        //[Inject] ILocalStorageService localStorageService { get; set; } = null!;
        [Inject] BookOperationsService BookOperationsService { get; set; } = null!;
        [Inject] FilesOperationsService FilesOperationsService { get; set; } = null!;

        [Parameter]
        public string BookId { get; set; }

        private IList<BookSection> Sections { get; set; } = new List<BookSection>();
        //private IEnumerable<BookContent> Content { get; set; } = new List<BookContent>();
        private string _actualPage = null!;
        private MarkupString html;
        private int maxHeight = 560;
        private int _actualPageNumber = 1;
        private int pagesCount = 0;
        private int actualSectionPagesCount = 0;
        private int currentSectionNumber;
        private List<string> pages = new List<string>();
        private bool _isLoading;
        private ElementReference elementReference;

        protected override async Task OnInitializedAsync()
        {
            //Stopwatch stopwatch = Stopwatch.StartNew();
            _isLoading = true;
            //string base64 = await FilesOperationsService.GetBookFile(Guid.Parse(BookId));       //TODO move this code to Library and save edited html in database {
            //byte[] bytes = Convert.FromBase64String(base64);

            //EpubBookRef? epubBookRef = await EpubReader.OpenBookAsync(new MemoryStream(bytes));
            //var list = await epubBookRef.GetReadingOrderAsync();

            //var bytesHtml = await list.First().ReadContentAsBytesAsync();
            //var htmlParser = new HtmlParser();

            //var content = "";

            //When book opens, get all prepared sections from database
            //Display first section, when user move to the next section, draw and display the section from prepared list.
            //Get all sections as IEnumerable

            Sections = await BookOperationsService.GetBookSections(Guid.Parse(BookId));
            var listOfStrings = new List<string>();
            foreach (var section in Sections)
            {
                //pagesCount += SeparateHtmlToPages(await JS.InvokeAsync<int>("setIframeDocument", section.Text));
                listOfStrings.Add(section.Text);
            }
            await JS.InvokeVoidAsync("initializeBookContainer");

            //pagesCount = await JS.InvokeAsync<int>("countPagesOfBook", listOfStrings);

            //await JS.InvokeAsync<int>("initializeBookContainer", Sections[2].Text);
            //await JS.InvokeVoidAsync("setClone");
            //await JS.InvokeAsync<int>("separateHtmlOnPages", Sections[2].Text);

            await JS.InvokeVoidAsync("setClone");
            foreach (var item in Sections)      //TODO what if section will be less then one page?
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

            //foreach (var section in Sections)
            //{
            //    Stopwatch stopwatch = Stopwatch.StartNew();
            //    DivideAndSetHtml(section.Text);
            //    //await JS.InvokeVoidAsync("clearIframeDocument");
            //    stopwatch.Stop();
            //    Console.WriteLine(stopwatch.ElapsedMilliseconds);
            //}

            currentSectionNumber = 0;
            actualSectionPagesCount = await JS.InvokeAsync<int>("divideAndSetHtml", Sections.First().Text);
            //await DivideAndSetHtml(Sections[1].Text);

            //TODO show a whole book, not one section
            //await JS.InvokeVoidAsync("clearIframeDocument");

            //pagesCount = await JS.InvokeAsync<int>("countPagesOfBook", listOfStrings);        //Move this code to C#

            _isLoading = false;
            await base.OnInitializedAsync();
            //stopwatch.Stop();
            //Console.WriteLine(stopwatch.ElapsedMilliseconds);
        }

        private async Task DivideAndSetHtml(string content)
        {
            //Getting iframeDocument from webpage
            var context = BrowsingContext.New(Configuration.Default);
            var iframeDocumentHtml = await JS.InvokeAsync<string>("getIframeDocument");
            var document = await context.OpenAsync(req => req.Content(iframeDocumentHtml));

            //Parse html of book section (content)
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(content);
            if (document.Head is not null && doc.Head is not null)
            {
                document.Head.InnerHtml = doc.Head.InnerHtml;
                document.Body.InnerHtml = doc.Body.InnerHtml;
            }

            //Add class to book section to hide scrollbar
            IHtmlLinkElement link = document.CreateElement<IHtmlLinkElement>();
            link.Href = "http://localhost:5284/css/container.css";
            link.Relation = "stylesheet";
            link.Type = "text/css";
            document.Head.AppendChild(link);

            //Set iframeDocument with JS and return its offsetHeight
            string html = document.ToHtml();
            var iframeBodyOffsetHeight = await JS.InvokeAsync<int>("setIframeDocument", html);

            //Dividing html on columns (pages) and setting additional style parameters
            actualSectionPagesCount = SeparateHtmlToPages(iframeBodyOffsetHeight);
            document.Body.SetStyle($"padding: 10; margin: 0; width: {900 * actualSectionPagesCount}; height: {maxHeight}; column-count: {actualSectionPagesCount}");
            html = document.ToHtml();
            await JS.InvokeAsync<int>("setIframeDocument", html);
            //return actualSectionPagesCount;
        }

        private int SeparateHtmlToPages(int offsetHeight)
        {
            return (int)Math.Ceiling((double)offsetHeight / maxHeight);
        }

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    await base.OnAfterRenderAsync(firstRender);
        //}

        private int GetIndexOfActualPage()
        {
            return pages.IndexOf(_actualPage);
        }

        private async void ChangePage(int pageNumber)
        {
            _actualPageNumber = pageNumber;
            await JS.InvokeVoidAsync("setActualPage", pages[_actualPageNumber - 1]);
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
                //Stopwatch stopwatch = Stopwatch.StartNew();
                await JS.InvokeAsync<int>("divideAndSetHtml", Sections[currentSectionNumber].Text);
                //stopwatch.Stop();
                //Console.WriteLine(stopwatch.ElapsedMilliseconds);
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

        //For pdf       //For pdf maybe I can use freespire.pdf

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
