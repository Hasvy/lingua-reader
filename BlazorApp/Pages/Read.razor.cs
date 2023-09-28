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
using System.Reflection.Metadata;
using VersOne.Epub;

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
        private int _actualPageNumber = 1;
        private IHtmlDocument _htmlDocument;
        private IDocument iframeDocument;
        private string? iframeBodyHtml;
        private int pagesCount;
        private List<string> pages = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            byte[] bytes = await FilesOperationsService.GetBookFile(Guid.Parse(BookId));

            string tmpFileName = "activeBook.epub";     //Work with the file, get sections from it, show them on page, maybe use MarkupString instead of iframe
            string tmpFilePath = Directory.GetCurrentDirectory() + tmpFileName;
            File.WriteAllBytes(tmpFilePath, bytes);

            Sections = await BookOperationsService.GetBookSections(Guid.Parse(BookId));
            Content = await BookOperationsService.GetBookContent(Guid.Parse(BookId));

            //  await JS.InvokeVoidAsync("setupReadingPage");
                                            //TODO fix sections and show whole book.
            string css = string.Empty;      //TODO Fix it to comfort reading
            foreach (var item in Content)
            {
                css += item.Content;
            }
            pagesCount = await JS.InvokeAsync<int>("initializeBookContainer", Sections.First().Text, css);
            //await JS.InvokeAsync<string?>("addStyle", css);

            //await JS.InvokeVoidAsync("setActualPage", pages[_actualPageNumber - 1]);
            await base.OnInitializedAsync();
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
