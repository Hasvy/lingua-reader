using EmailService;
using Microsoft.AspNetCore.Components;
using Objects.Dto.Authentication;
using Services.Authentication;

namespace BlazorApp.Pages.Authentication
{
    public partial class Login : ComponentBase
    {
        private UserForAuthenticationDto _userForAuthentication = new UserForAuthenticationDto();
        private bool _isPasswordVisible = false;
        [Inject] public IAuthenticationService AuthenticationService { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        public bool ShowAuthError { get; set; }
        private AuthResponseDto? Result { get; set; }
        private bool _isLogging { get; set; }
        public async Task ExecuteLogin()
        {
            ShowAuthError = false;
            _isLogging = true;
            var result = await AuthenticationService.Login(_userForAuthentication);
            if (!result.IsAuthSuccessful)
            {
                Result = new AuthResponseDto();
                Result.ErrorMessage = result.ErrorMessage;
                Result.Url = result.Url;
                Result.UrlText = result.UrlText;
                ShowAuthError = true;
            }
            else
            {
                NavigationManager.NavigateTo("/Reading");
            }
            _isLogging = false;
        }

        public void GoToForgotPassword()
        {
            NavigationManager.NavigateTo("/ForgotPassword");
        }

        void TogglePassword()
        {
            _isPasswordVisible = !_isPasswordVisible;
        }
    }
}
