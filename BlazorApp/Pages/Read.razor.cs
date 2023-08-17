using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Blazored.LocalStorage;
using Objects.Components.Library;

namespace BlazorApp.Pages
{
    public partial class Read : ComponentBase
    {
        [Inject] ILocalStorageService localStorage { get; set; } = null!;

        [Parameter]
        public string? BookId { get; set; }
        private List<string> Text { get; set; } = new List<string>();
        private string _actualPage = null!;
        private int _actualPageNumber;

        protected override async Task OnInitializedAsync()
        {
            //Get Book form localStorage
            var stringCover = await localStorage.GetItemAsync<string>(BookId);
            var cover = JsonConvert.DeserializeObject<BookCover>(stringCover);

            if (cover != null)
            {
                var stringText = await localStorage.GetItemAsync<string>(cover.TextId.ToString());
                Text = JsonConvert.DeserializeObject<List<string>>(stringText);
            }

            if (Text != null)
            {
                _actualPage = Text.First();
            }

            _actualPageNumber = GetIndexOfActualPage() + 1;

            await base.OnInitializedAsync();
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
