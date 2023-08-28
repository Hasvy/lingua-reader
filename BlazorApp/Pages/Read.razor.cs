using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Blazored.LocalStorage;
using Objects.Components.Library;
using Objects;
using AngleSharp.Html.Parser;
using AngleSharp.Html.Dom;
using System.Reflection.Metadata;

namespace BlazorApp.Pages
{
    public partial class Read : ComponentBase
    {
        [Inject] ILocalStorageService localStorage { get; set; } = null!;
        [Inject] HtmlParser htmlParser { get; set; } = null!;

        [Parameter]
        public string? BookId { get; set; }
        private List<string> Text { get; set; } = new List<string>();
        private List<string> Sections { get; set; } = new List<string>();
        private string _actualPage = null!;
        private int _actualPageNumber;
        private ElementReference html;

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
                    Text = JsonConvert.DeserializeObject<List<string>>(stringText);
                }
            }

            if (Text != null && Text.Any())
            {
                _actualPage = Text.First();
            }

            _actualPageNumber = GetIndexOfActualPage() + 1;

            await base.OnInitializedAsync();
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

        private async Task<List<string>> ParseEpubBook(string htmlContent)
        {
            List<string> extractedContent = new List<string>();
            htmlContent = htmlContent.Replace("<a id=\"p1\"></a>", "<a id=\"p1\">abc</a>");
            var document = htmlParser.ParseDocument(htmlContent);
            var currentElement = document.GetElementById("p1");

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
            return extractedContent;
        }

        private int GetIndexOfActualPage()
        {
            return Text.IndexOf(_actualPage);
        }

        private void NextPage()
        {
            if (_actualPageNumber != Text.Count)
            {
                _actualPage = Text[GetIndexOfActualPage() + 1];
                _actualPageNumber++;
            }
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
