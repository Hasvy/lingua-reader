using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Blazored.LocalStorage;
using Objects.Components.Library;
using Objects;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using System.Reflection.Metadata;
using static System.Collections.Specialized.BitVector32;
using Microsoft.JSInterop;
using AngleSharp.Common;
using AngleSharp.Dom;
using AngleSharp.Text;
using System.Text;
using AngleSharp;

namespace BlazorApp.Pages
{
    public partial class Read : ComponentBase
    {
        [Inject] ILocalStorageService localStorage { get; set; } = null!;
        [Inject] HtmlParser htmlParser { get; set; } = null!;
        [Inject] IJSRuntime JS { get; set; } = null!;

        [Parameter]
        public string? BookId { get; set; }
        private List<string> Text { get; set; } = new List<string>();
        private List<IElement> AllBodyElements { get; set; } = new List<IElement>();
        private List<IElement> SelectedBodyElements { get; set; } = new List<IElement>();
        private List<string> Sections { get; set; } = new List<string>();
        private string _actualPage = null!;
        private string _head = null!;
        private string _body = null!;
        private int _actualPageNumber = 1;
        private IHtmlDocument _htmlDocument;
        private IDocument iframeDocument;
        private int elementIndex = 0;
        private string? iframeBodyHtml;

        private DotNetObjectReference<JSInterop> _jsReference;

        protected override async Task OnInitializedAsync()
        {
            //Get Book form localStorage
            var stringCover = await localStorage.GetItemAsync<string>(BookId);
            var cover = JsonConvert.DeserializeObject<BookCover>(stringCover);

            if (cover != null)
            {
                var stringText = await localStorage.GetItemAsync<string>(cover.TextId.ToString());
                if (cover.Format == ConstBookFormats.pdf)
                {
                    Text = JsonConvert.DeserializeObject<List<string>>(stringText);
                }
                if (cover.Format == ConstBookFormats.epub)
                {
                    Sections = JsonConvert.DeserializeObject<List<string>>(stringText);         //TODO Fix read page size, and show text by pages
                    await JS.InvokeVoidAsync("setupReadingPage");
                }
            }

            //Test
            if (Sections != null && Sections.Any())
            {
                _actualPage = Sections[3];
                await ParseEpubBookToList();
                await SplitToPages();
                _actualPageNumber = 1;
                await JS.InvokeVoidAsync("setActualPage", pages[_actualPageNumber - 1]);
                //await SetActualPageText();
            }

            await base.OnInitializedAsync();
        }

        private async Task SetActualPageText()
        {
            //bool isHeightInBorders = true;
            //await JS.InvokeVoidAsync("getContainerElement");
            //while (isHeightInBorders && elementIndex <= AllBodyElements.Count)      //Or change mechanism to set all pages while initializing, and then show them.
            //{
            //    iframeDocument.Body.AppendChild(AllBodyElements[elementIndex]);
            //    iframeBodyHtml = iframeDocument.Body.ToHtml();
            //    isHeightInBorders = await JS.InvokeAsync<bool>("getTextContainer", iframeBodyHtml);
            //    if (isHeightInBorders)
            //    {
            //        elementIndex++;
            //    }
            //    else
            //    {
            //        elementIndex--;
            //    }
            //}
            //iframeDocument.Body.InnerHtml = "";
        }

        int firstIndex = 0;
        int lastIndex = 0;
        private List<string> pages = new List<string>();
        private async Task SplitToPages()
        {
            while (lastIndex < AllBodyElements.Count)
            {
                bool isHeightInBorders = true;
                await JS.InvokeVoidAsync("clearContainerElement");
                var page = new List<IElement>();
                while (isHeightInBorders && lastIndex < AllBodyElements.Count)
                {
                    iframeDocument.Body.AppendChild(AllBodyElements[lastIndex]);
                    iframeBodyHtml = iframeDocument.Body.ToHtml();
                    isHeightInBorders = await JS.InvokeAsync<bool>("getTextContainer", iframeBodyHtml);
                    if (isHeightInBorders)
                    {
                        page.Add(AllBodyElements[lastIndex]);
                        lastIndex++;
                    }
                }
                iframeDocument.Body.InnerHtml = "";

                string html = string.Empty;
                foreach (var item in page)
                {
                    html += item.OuterHtml;
                }
                page.Clear();
                pages.Add(html);
            }
        }

        private async Task<List<string>> ParseEpubBookToList()
        {
            var parser = new HtmlParser();
            _htmlDocument = await parser.ParseDocumentAsync(Sections[3]);
            _head = _htmlDocument.Head.OuterHtml;
            //_body = _htmlDocument.Body.OuterHtml;       //Without inner html, only body tag
            AllBodyElements = _htmlDocument.Body.Children.ToList();

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);
            var contextParser = context.GetService<IHtmlParser>();

            iframeDocument = await context.OpenNewAsync();
            tempDocument = await context.OpenNewAsync();
            iframeDocument.Head.OuterHtml = _htmlDocument.Head.OuterHtml;
            iframeDocument.Body.OuterHtml = _htmlDocument.Body.OuterHtml;
            iframeDocument.Body.InnerHtml = "";
            //List<string> extractedContent = new List<string>();
            //htmlContent = htmlContent.Replace("<a id=\"p1\"></a>", "<a id=\"p1\">abc</a>");
            //var document = htmlParser.ParseDocument(htmlContent);
            //var currentElement = document.GetElementById("p1");

            //for (int i = 1; i <= length; i++)
            //{
            //    if (currentElement != null)
            //    {
            //        var nextElement = currentElement.NextElementSibling;
            //        while (nextElement != null && nextElement.LocalName != "a" && !nextElement.Id.StartsWith("p"))
            //        {
            //            extractedContent.Add(nextElement.OuterHtml);
            //            nextElement = nextElement.NextElementSibling;
            //        }
            //    }
            //}
            return new List<string>();
        }

        private int GetIndexOfActualPage()
        {
            return pages.IndexOf(_actualPage);
        }

        private async void NextPage()
        {
            await SetActualPageText();
            if (_actualPageNumber != pages.Count)
            {
                //_actualPage = pages[GetIndexOfActualPage() + 1];
                _actualPageNumber++;
                await JS.InvokeVoidAsync("setActualPage", pages[_actualPageNumber - 1]);
            }
        }

        private async void PreviousPage()
        {
            if (_actualPageNumber != 1)
            {
                //_actualPage = pages[GetIndexOfActualPage() - 1];
                _actualPageNumber--;
                await JS.InvokeVoidAsync("setActualPage", pages[_actualPageNumber - 1]);
            }
        }

        //For pdf

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
