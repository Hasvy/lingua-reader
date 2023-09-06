﻿using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using Objects;
using Objects.Components.Library;
using System.Reflection.Metadata;

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
        private List<string> pages = new List<string>();

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
                    Sections = JsonConvert.DeserializeObject<List<string>>(stringText);
                    await JS.InvokeVoidAsync("setupReadingPage");
                }
            }

            if (Sections != null && Sections.Any())     //TODO fix sections and show whole book.
            {
                _actualPage = Sections[3];
                await ParseEpubBookToList();
                await SplitToPages();
                _actualPageNumber = 1;
                await JS.InvokeVoidAsync("setActualPage", pages[_actualPageNumber - 1]);
            }

            await base.OnInitializedAsync();
        }

        //private async Task SetActualPageText()                //Another variant of paging, setup every page right before display.
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

        private async Task SplitToPages()       //Cleanup and rename vars
        {
            int lastIndex = 0;
            string html;

            while (lastIndex < AllBodyElements.Count)
            {
                bool isHeightInBorders = true;
                await JS.InvokeVoidAsync("clearContainerElement");
                var page = new List<IElement>();
                while (isHeightInBorders && lastIndex < AllBodyElements.Count)
                {
                    iframeDocument.Body.AppendChild(AllBodyElements[lastIndex]);
                    iframeBodyHtml = iframeDocument.Body.ToHtml();
                    isHeightInBorders = await JS.InvokeAsync<bool>("checkContainerHeight", iframeBodyHtml);
                    if (isHeightInBorders)
                    {
                        page.Add(AllBodyElements[lastIndex]);
                        lastIndex++;
                    }
                }
                iframeDocument.Body.InnerHtml = "";

                html = string.Empty;
                foreach (var item in page)
                {
                    html += item.OuterHtml;
                }
                page.Clear();
                pages.Add(html);
            }
        }

        private async Task ParseEpubBookToList()
        {
            var parser = new HtmlParser();
            _htmlDocument = await parser.ParseDocumentAsync(Sections[3]);
            _head = _htmlDocument.Head.OuterHtml;
            AllBodyElements = _htmlDocument.Body.Children.ToList();

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            iframeDocument = await context.OpenNewAsync();
            iframeDocument.Head.OuterHtml = _htmlDocument.Head.OuterHtml;
            iframeDocument.Body.OuterHtml = _htmlDocument.Body.OuterHtml;
            iframeDocument.Body.InnerHtml = "";
        }

        private int GetIndexOfActualPage()
        {
            return pages.IndexOf(_actualPage);
        }

        private async void NextPage()
        {
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
                //_actualPage = pages[GetIndexOfActualPage()];
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
