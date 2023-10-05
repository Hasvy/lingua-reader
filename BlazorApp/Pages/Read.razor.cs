using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Reflection.Metadata;

namespace BlazorApp.Pages
{
    public partial class Read : ComponentBase
    {
        [Inject] ILocalStorageService LocalStorageService { get; set; } = null!;
        [Parameter] public string BookId { get; set; } = null!;
        private string BookFormat { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            BookFormat = await LocalStorageService.GetItemAsStringAsync("bookFormat");      //19-30 msec
            await base.OnInitializedAsync();
        }   
    }
}
