using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Objects.Constants;

namespace BlazorApp.Pages
{
    public partial class Settings : ComponentBase
    {
        [Inject] ILocalStorageService LocalStorageService { get; set; } = null!;
        private string UserMainLang = ConstLanguages.Czech;

        protected override async Task OnInitializedAsync()
        {
            UserMainLang = await LocalStorageService.GetItemAsStringAsync("UserMainLang");
            await base.OnInitializedAsync();
        }

        private async void ChangeUserLang(string chosenLang)
        {
            //Todo save language in user settings
            await LocalStorageService.SetItemAsStringAsync("UserMainLang", chosenLang);
        }
    }
}
