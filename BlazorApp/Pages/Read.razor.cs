using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Services;
using System.Diagnostics;
using VersOne.Epub;

namespace BlazorApp.Pages
{
    public partial class Read : ComponentBase
    {
        [Inject] HtmlParser htmlParser { get; set; } = null!;
        [Inject] IJSRuntime JS { get; set; } = null!;
        //[Inject] ILocalStorageService localStorageService { get; set; } = null!;
        //[Inject] BookOperationsService BookOperationsService { get; set; } = null!;
        [Inject] FilesOperationsService FilesOperationsService { get; set; } = null!;

        [Parameter]
        public string BookId { get; set; }

        //private IEnumerable<BookSection> Sections { get; set; } = new List<BookSection>();
        //private IEnumerable<BookContent> Content { get; set; } = new List<BookContent>();
        private string _actualPage = null!;
        private MarkupString html;
        private int maxHeight = 560;
        private int _actualPageNumber = 1;
        private int pagesCount;
        private int actualSectionPagesCount = 0;
        private List<string> pages = new List<string>();
        private bool _isLoading;
        private ElementReference elementReference;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;

            string base64 = await FilesOperationsService.GetBookFile(Guid.Parse(BookId));
            byte[] bytes = Convert.FromBase64String(base64);

            EpubBookRef? epubBookRef = await EpubReader.OpenBookAsync(new MemoryStream(bytes));
            var list = await epubBookRef.GetReadingOrderAsync();

            var bytesHtml = await list.First().ReadContentAsBytesAsync();
            var htmlParser = new HtmlParser();
            var sectionHtml = htmlParser.ParseDocument(await list[3].ReadContentAsync());
            var cssEls = sectionHtml.QuerySelectorAll("link[rel='stylesheet']");
            var imgEls = sectionHtml.QuerySelectorAll("img");

            foreach (var imgEl in imgEls)
            {
                string src = imgEl.GetAttribute("src");
                var img = epubBookRef.Content.Images.Local.SingleOrDefault(i => Path.GetFileName(i.FilePath) == Path.GetFileName(src));
                if (img is not null)
                {
                    byte[] imgBytes = await img.ReadContentAsBytesAsync();
                    string imgBase64 = Convert.ToBase64String(imgBytes);
                    imgEl.SetAttribute("src", $"data:image/{Path.GetExtension(src).Replace(".", "")};base64," + imgBase64);
                }
            }
            foreach (var cssEl in cssEls)
            {
                string src = cssEl.GetAttribute("href");
                var css = epubBookRef.Content.Css.Local.SingleOrDefault(c => Path.GetFileName(c.FilePath) == Path.GetFileName(src));
                if (css is not null)
                {
                    byte[] imgBytes = await css.ReadContentAsBytesAsync();
                    string imgBase64 = Convert.ToBase64String(imgBytes);
                    cssEl.SetAttribute("href", $"data:text/{Path.GetExtension(src).Replace(".", "")};base64," + imgBase64);
                }
            }

            var content = sectionHtml.ToHtml();        //TODO move this code to Library and save edited html in database


            Stopwatch stopwatch = new Stopwatch();

            //C#
            stopwatch.Start();
            Method(content);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            await JS.InvokeVoidAsync("clearIframeDocument");

            //JS
            stopwatch.Restart();
            actualSectionPagesCount = await JS.InvokeAsync<int>("initializeBookContainer", content);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);

            //pagesCount = await JS.InvokeAsync<int>("countPagesOfBook", listOfStrings);

            _isLoading = false;
            await base.OnInitializedAsync();

            //string tmpFileName = "activeBook.epub";     //Work with the file, get sections from it, show them on page, maybe use MarkupString instead of iframe
            //string path = "wwwroot/Uploads/";
            //string tmpFilePath = Directory.GetCurrentDirectory() + tmpFileName;
            //var info = Directory.CreateDirectory(path);
            //File.Create(tmpFilePath);
            //File.WriteAllBytes(tmpFilePath, bytes);
            //EpubConverter converter = new EpubConverter();
            //converter.Convert(tmpFilePath);
        }

        private async void Method(string content)
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
        }

        private int SeparateHtmlToPages(int offsetHeight)
        {
            return (int)Math.Ceiling((double)offsetHeight / maxHeight);
        }

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
            
        //    await base.OnAfterRenderAsync(firstRender);
        //}

        //private async Task ParseBookSection(string section)     //Separate head and body, now it works in JS
        //{
        //    var parser = new HtmlParser();
        //    _htmlDocument = await parser.ParseDocumentAsync(section);
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
                await JS.InvokeVoidAsync("nextPage");
            }
        }

        private async void PreviousPage()
        {
            if (_actualPageNumber != 1)
            {
                _actualPageNumber--;
                await JS.InvokeVoidAsync("previousPage");
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
