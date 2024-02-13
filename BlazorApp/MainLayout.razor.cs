using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp
{
    public partial class MainLayout
    {
        [CascadingParameter] protected Task<AuthenticationState> AuthStat { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        protected async override Task OnInitializedAsync()
        {
            base.OnInitialized();
            var user = (await AuthStat).User;
            if (!user.Identity.IsAuthenticated)
            {
                NavigationManager.NavigateTo("/Login");
            }
        }
    }
}
