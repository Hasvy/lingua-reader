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
using Objects.Entities.Books.EpubBook;
using Services;
using System.Reflection.Metadata;

namespace BlazorApp.Pages
{
    public partial class OldRead : ComponentBase
    {
        [Inject] HtmlParser htmlParser { get; set; } = null!;
        [Inject] IJSRuntime JS { get; set; } = null!;
        [Inject] BookOperationsService BookOperationsService { get; set; } = null!;

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
        private int _actualPageNumber = 1;
        private IHtmlDocument _htmlDocument;
        private IDocument iframeDocument;
        private string? iframeBodyHtml;
        private List<string> pages = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            Sections = await BookOperationsService.GetBookSections(Guid.Parse(BookId));
            Content = await BookOperationsService.GetBookContent(Guid.Parse(BookId));

            //  await JS.InvokeVoidAsync("setupReadingPage");
            await JS.InvokeAsync<string?>("initializeBookContainer", Sections.First().Text);        //TODO fix a bug when open a book right after upload Sequence contains no elements
                                            //TODO fix sections and show whole book.
            string css = string.Empty;      //TODO Fix it to comfort reading
            foreach (var item in Content)
            {
                css += item.Content;
            }
            await JS.InvokeAsync<string?>("addStyle", css);

            //await JS.InvokeVoidAsync("setActualPage", pages[_actualPageNumber - 1]);
            await base.OnInitializedAsync();
        }

        //private async Task ParseBookSection(string section)     //Separate head and body, now it works in JS
        //{
        //    var parser = new HtmlParser();
        //    _htmlDocument = await parser.ParseDocumentAsync(section);
        //}



        //Work with page

        //private async Task<List<IElement>> ParseBookSectionToList(string section)
        //{
        //    var parser = new HtmlParser();
        //    _htmlDocument = await parser.ParseDocumentAsync(section);
        //    _head = _htmlDocument.Head.OuterHtml;
        //    var bodyElements = _htmlDocument.Body.Children.ToList();

        //    var config = Configuration.Default.WithDefaultLoader();
        //    var context = BrowsingContext.New(config);

        //    iframeDocument = await context.OpenNewAsync();
        //    iframeDocument.Head.OuterHtml = _htmlDocument.Head.OuterHtml;
        //    iframeDocument.Body.OuterHtml = _htmlDocument.Body.OuterHtml;
        //    iframeDocument.Body.InnerHtml = "";
            
        //    return bodyElements;
        //}

        //private async Task SplitToPages(List<IElement> elements)       //Cleanup and rename vars
        //{
        //    int lastIndex = 0;
        //    string html;

        //    while (lastIndex < elements.Count)      //TODO fix test2.epub sets two times. TODO separate files and classes in ReadPdf, ReadEpub etc. Create diagram
        //    {
        //        bool isHeightInBorders = true;
        //        await JS.InvokeVoidAsync("clearContainerElement");
        //        var page = new List<IElement>();
        //        while (isHeightInBorders && lastIndex < elements.Count)
        //        {
        //            iframeDocument.Body.AppendChild(elements[lastIndex]);
        //            iframeBodyHtml = iframeDocument.Body.ToHtml();
        //            isHeightInBorders = await JS.InvokeAsync<bool>("checkContainerHeight", iframeBodyHtml);
        //            if (isHeightInBorders)          //Is possible infinity cycle here   TODO fix if first element on page is heigher then borders
        //            {
        //                page.Add(elements[lastIndex]);
        //            }
        //            lastIndex++;
        //        }
        //        iframeDocument.Body.InnerHtml = "";

        //        html = string.Empty;
        //        foreach (var item in page)
        //        {
        //            html += item.OuterHtml;
        //        }
        //        page.Clear();
        //        pages.Add(html);
        //    }
        //}

        //private async Task SetActualPageText()        //Another variant of paging, setup every page right before display.
        //{
        //bool isHeightInBorders = true;
        //await JS.InvokeVoidAsync("getContainerElement");
        //while (isHeightInBorders && elementIndex <= AllBodyElements.Count)
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
            if (_actualPageNumber != pages.Count)
            {
                //_actualPage = pages[GetIndexOfActualPage() + 1];
                _actualPageNumber++;
                await JS.InvokeVoidAsync("nextPage");
            }
        }

        private async void PreviousPage()
        {
            if (_actualPageNumber != 1)
            {
                //_actualPage = pages[GetIndexOfActualPage()];
                _actualPageNumber--;
                await JS.InvokeVoidAsync("previousPage");
            }
        }

        //For list of pages
        //private async void NextPage()
        //{
        //    if (_actualPageNumber != pages.Count)
        //    {
        //        _actualPage = pages[GetIndexOfActualPage() + 1];
        //        _actualPageNumber++;
        //        await JS.InvokeVoidAsync("setActualPage", pages[_actualPageNumber - 1]);
        //    }
        //}

        //private async void PreviousPage()
        //{
        //    if (_actualPageNumber != 1)
        //    {
        //        _actualPage = pages[GetIndexOfActualPage()];
        //        _actualPageNumber--;
        //        await JS.InvokeVoidAsync("setActualPage", pages[_actualPageNumber - 1]);
        //    }
        //}

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
