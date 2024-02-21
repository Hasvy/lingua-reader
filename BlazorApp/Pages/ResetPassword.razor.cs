using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Objects.Dto;
using Services.Authentication;

namespace BlazorApp.Pages
{
    public partial class ResetPassword : ComponentBase
    {
        [Inject] public IAuthenticationService AuthenticationService { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        public bool ShowRegistrationErrors { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        private ResetPasswordDto _resetPasswordDto = new ResetPasswordDto();
        private bool _isPasswordVisible = false;
        private bool _isConfirmPasswordVisible = false;

        protected override void OnInitialized()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryParameters = QueryHelpers.ParseQuery(uri.Query);

            if (queryParameters.TryGetValue("token", out var token))
            {
                _resetPasswordDto.Token = token!;
            }
            if (queryParameters.TryGetValue("email", out var email))
            {
                _resetPasswordDto.Email = email!;
            }
            base.OnInitialized();
        }

        public async Task Reset()
        {
            ShowRegistrationErrors = false;
            var result = await AuthenticationService.ResetPassword(_resetPasswordDto);
            if (!result.IsSuccessfulRegistration)
            {
                Errors = result.Errors;
                ShowRegistrationErrors = true;
            }
            else
            {
                NavigationManager.NavigateTo("/ResetPasswordConfirmation/Success");
            }
        }

        void TogglePassword()
        {
            _isPasswordVisible = !_isPasswordVisible;
            _isConfirmPasswordVisible = !_isConfirmPasswordVisible;
        }
    }
}
