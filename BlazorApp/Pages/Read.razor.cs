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
        private int _actualPageNumber;
        private IHtmlDocument _htmlDocument;
        private IDocument iframeDocument;

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
                }
            }

            //Test
            if (Sections != null && Sections.Any())
            {
                _actualPage = Sections[3];
                ParseEpubBookToList();
            }

            

            //_actualPageNumber = GetIndexOfActualPage() + 1;
            await base.OnInitializedAsync();
        }

        [JSInvokable]
        private async Task GetActualPageText(List<string> sections)
        {

        }

        private async Task ReadHtml(List<string> sections)
        {
            var list = new List<IHtmlDocument>();
            var parser = new HtmlParser();
            foreach (var section in sections)
            {
                list.Add(await parser.ParseDocumentAsync(section));
            }
            foreach (var item in list)
            {
                if (item.Body != null)
                {
                    Text.Add(item.Body.InnerHtml);
                }
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
            return Text.IndexOf(_actualPage);
        }

        string? iframeHtml;
        private async void NextPage()
        {

            //JS.InvokeVoidAsync("bookPageChange");
            //Text.Add(_htmlDocument.Body.Children.ToList();

            //SelectedBodyElements.Add(AllBodyElements[_actualPageNumber]);
            iframeDocument.Body.AppendChild(AllBodyElements[_actualPageNumber]);
            iframeHtml = iframeDocument.ToHtml();


            await JS.InvokeVoidAsync("getTextContainer", iframeHtml);       //TODO handle page overflow, and add page changing
            //if (_actualPageNumber != Text.Count)
            //{
            //    _actualPage = Text[GetIndexOfActualPage() + 1];
                _actualPageNumber++;
            //}
        }

        private void PreviousPage()
        {
            if (_actualPageNumber != 1)
            {
                _actualPage = Text[GetIndexOfActualPage() - 1];
                _actualPageNumber--;
            }
        }
    }
}
