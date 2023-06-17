using Microsoft.AspNetCore.Components;
using Objects.Components.Library;

namespace BlazorApp.Pages
{
    public partial class Read : ComponentBase
    {
        [Parameter]
        public string? BookId { get; set; }

        Book Book { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //get Book
            await base.OnInitializedAsync();
        }
    }
}
