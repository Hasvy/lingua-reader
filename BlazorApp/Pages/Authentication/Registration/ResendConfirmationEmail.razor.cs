using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Objects.Dto.Authentication;
using Services.Authentication;

namespace BlazorApp.Pages.Authentication.Registration
{
    public partial class ResendConfirmationEmail : ComponentBase
    {
        [Inject] IAuthenticationService AuthenticationService { get; set; } = null!;
        [Inject] NavigationManager NavigationManager { get; set; } = null!;
        public string? Error { get; set; }
        private ConfirmEmailResponseDto? _confirmEmailResponseDto = null;
        protected async override Task OnInitializedAsync()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryParameters = QueryHelpers.ParseQuery(uri.Query);

            if (queryParameters.TryGetValue("email", out var email))
            {
                _confirmEmailResponseDto = await AuthenticationService.ResendConfirmationEmail(email);
            }

            await base.OnInitializedAsync();
        }
    }
}
