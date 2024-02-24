using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Objects.Dto.Authentication;
using Services.Authentication;

namespace BlazorApp.Pages.Authentication.Registration
{
    public partial class ConfirmEmail : ComponentBase
    {
        [Inject] public IAuthenticationService AuthenticationService { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        private ConfirmEmailDto _confirmEmailDto = new ConfirmEmailDto();
        private ConfirmEmailResponseDto? _confirmEmailResponseDto = null;
        protected override async Task OnInitializedAsync()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryParameters = QueryHelpers.ParseQuery(uri.Query);

            if (queryParameters.TryGetValue("token", out var token))
            {
                _confirmEmailDto.Token = token!;
            }
            if (queryParameters.TryGetValue("email", out var email))
            {
                _confirmEmailDto.Email = email!;
            }

            _confirmEmailResponseDto = await AuthenticationService.ConfirmEmail(_confirmEmailDto);

            await base.OnInitializedAsync();
        }
    }
}
