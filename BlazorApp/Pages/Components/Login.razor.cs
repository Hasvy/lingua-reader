using EmailService;
using Microsoft.AspNetCore.Components;
using Objects.Dto;
using Services.Authentication;

namespace BlazorApp.Pages.Components
{
    public partial class Login : ComponentBase
    {
        private UserForAuthenticationDto _userForAuthentication = new UserForAuthenticationDto();
        [Inject] public IAuthenticationService AuthenticationService { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        public bool ShowAuthError { get; set; }
        public string? Error { get; set; }
        public async Task ExecuteLogin()
        {
            ShowAuthError = false;
            var result = await AuthenticationService.Login(_userForAuthentication);
            if (!result.IsAuthSuccessful)
            {
                Error = result.ErrorMessage;
                ShowAuthError = true;
            }
            else
            {
                NavigationManager.NavigateTo("/");
            }
        }

        public void GoToForgotPassword()
        {
            NavigationManager.NavigateTo("/ForgotPassword");
        }
    }
}
