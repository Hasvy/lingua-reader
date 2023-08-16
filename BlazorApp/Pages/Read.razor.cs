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

            //if (cover != null)
            //{
            //   Text = await localStorage.GetItemAsync<string>(cover.TextId.ToString());
            //}

            await base.OnInitializedAsync();
        }
    }
}
