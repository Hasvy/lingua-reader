using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Objects;
using Objects.Entities;
using Services;
using System.Text.Json;
using VersOne.Epub;
using EpubSharp.Format;
using EpubSharp;

namespace BlazorApp.Pages
{
    public partial class Read : ComponentBase
    {
        [Inject] HtmlParser htmlParser { get; set; } = null!;
        [Inject] IJSRuntime JS { get; set; } = null!;
        [Inject] ILocalStorageService localStorageService { get; set; } = null!;
        [Inject] BookOperationsService BookOperationsService { get; set; } = null!;
        [Inject] FilesOperationsService FilesOperationsService { get; set; } = null!;

        [Parameter]
        public string BookId { get; set; }
        private List<string> Text { get; set; } = new List<string>();
        private List<IElement> AllBodyElements { get; set; } = new List<IElement>();
        private List<IElement> SelectedBodyElements { get; set; } = new List<IElement>();
        private IEnumerable<BookSection> Sections { get; set; } = new List<BookSection>();
        private IEnumerable<BookContent> Content { get; set; } = new List<BookContent>();
        private string _actualPage = null!;
        private string _head = null!;
        private string _body = null!;
        private string htmlPath = string.Empty;
        private MarkupString html;
        private int _actualPageNumber = 1;
        private IHtmlDocument _htmlDocument;
        private IDocument iframeDocument;
        private string? iframeBodyHtml;
        private int actualSectionPagesCount;
        private int pagesCount;
        private List<string> pages = new List<string>();
        private bool _isLoading;
        private ElementReference elementReference;

        protected override async Task OnInitializedAsync()
        {
            _isLoading = true;
            string base64 = await FilesOperationsService.GetBookFile(Guid.Parse(BookId));
            byte[] bytes = Convert.FromBase64String(base64);
            string tmpFileName = "activeBook.epub";     //Work with the file, get sections from it, show them on page, maybe use MarkupString instead of iframe
            //string path = "wwwroot/Uploads/";
            string tmpFilePath = Directory.GetCurrentDirectory() + tmpFileName;

            //var info = Directory.CreateDirectory(path);
            File.Create(tmpFilePath);
            File.WriteAllBytes(tmpFilePath, bytes);
            EpubConverter converter = new EpubConverter();
            //converter.Convert(tmpFilePath);

            EpubBookRef? epubBookRef = await VersOne.Epub.EpubReader.OpenBookAsync(tmpFilePath);        //TODO change file to stream
            //var filepath = epubBookRef.FilePath;
            var list = await epubBookRef.GetReadingOrderAsync();
            var bytesHtml = await list.First().ReadContentAsBytesAsync();

            var htmlParser = new HtmlParser();
            var sectionHtml = htmlParser.ParseDocument(await list[3].ReadContentAsync());
            var cssEls = sectionHtml.QuerySelectorAll("link[rel='stylesheet']");
            var imgEls = sectionHtml.QuerySelectorAll("img");
            foreach ( var imgEl in imgEls )
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
            //html = new MarkupString(sectionHtml.ToHtml());
            //JS to C#
            var aaa = elementReference.Context;
            var context = BrowsingContext.New(Configuration.Default);
            //var document = await context.OpenAsync(req => req.Content());

            actualSectionPagesCount = await JS.InvokeAsync<int>("initializeBookContainer", content);
            var listOfStrings = new List<string>();
            foreach (var item in list)
            {
                listOfStrings.Add(await item.ReadContentAsync());
            }
            pagesCount = await JS.InvokeAsync<int>("countPagesOfBook", listOfStrings);

            _isLoading = false;
            await base.OnInitializedAsync();

            //htmlPath = "/tmp/OEBPS/Text/Section0002.xhtml";
            //htmlPath = "/html.html";
            //File.Create(htmlPath);
            //using (var stream = System.IO.File.Create(htmlPath))
            //{
            //    stream.Write(bytesHtml, 0, bytesHtml.Length);
            //}

            //string rootpath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot");
            //var abc = File.Exists(rootpath + "index.html");
            //var ccc = File.Exists("/BlazorHostedSample/Server/bin/Release/{TFM}/publish/wwwroot/index.html");
            //var bb = File.Exists(htmlPath);

            //var dirs = Directory.GetDirectories("../");
            //var root = Directory.GetDirectoryRoot("../");
            //var ss = Directory.GetParent("../");

            //foreach (string file in Directory.EnumerateFiles("../", "*.*", SearchOption.AllDirectories))
            //{
            //    Console.WriteLine(file);
            //}

            //Use cloud storage?

            //var content = await File.ReadAllTextAsync(htmlPath);

            //var content = epubBookRef.Content;
            //content.Html.Local.First().

            //var guide = epubBookRef.Schema.Package.Guide.Items;

            //var list = content.Html.Local;
            //var activeFile = list[3];


            //var doc = new Document();
            //var element = new HtmlElement();


            //StreamWriter textWriter = File.CreateText("/newfile.epub");
            //System.Net.WebUtility.HtmlEncode(activeFile.ReadContent(), output: textWriter);
            //byte[] htmlBytes = await activeFile.ReadContentAsBytesAsync();
            //string base64Html = Convert.ToBase64String(htmlBytes);
            //dataUri = "data:text/html;base64," + base64Html;

            //Sections = await BookOperationsService.GetBookSections(Guid.Parse(BookId));
            //Content = await BookOperationsService.GetBookContent(Guid.Parse(BookId));

            ////  await JS.InvokeVoidAsync("setupReadingPage");
            //                                //TODO fix sections and show whole book.
            //string css = string.Empty;      //TODO Fix it to comfort reading
            //foreach (var item in Content)
            //{
            //    css += item.Content;
            //}
            //pagesCount = await JS.InvokeAsync<int>("initializeBookContainer", Sections.First().Text, css);
            ////await JS.InvokeAsync<string?>("addStyle", css);

            ////await JS.InvokeVoidAsync("setActualPage", pages[_actualPageNumber - 1]);
        }

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
