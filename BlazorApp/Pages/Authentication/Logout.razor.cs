using Microsoft.AspNetCore.Components;
using Services.Authentication;

namespace BlazorApp.Pages.Authentication
{
    public partial class Logout : ComponentBase
    {
        [Inject] public IAuthenticationService AuthenticationService { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        protected override async Task OnInitializedAsync()
        {
            await AuthenticationService.Logout();
            NavigationManager.NavigateTo("/");
        }
    }
}
