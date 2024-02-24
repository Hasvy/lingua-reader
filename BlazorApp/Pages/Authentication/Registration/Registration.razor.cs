using Microsoft.AspNetCore.Components;
using Objects.Dto.Authentication;
using Services.Authentication;

namespace BlazorApp.Pages.Authentication.Registration
{
    public partial class Registration : ComponentBase
    {
        [Inject] public IAuthenticationService AuthenticationService { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        public bool ShowRegistrationErrors { get; set; }
        public IEnumerable<string>? Errors { get; set; }
        private UserForRegistrationDto _userForRegistration = new UserForRegistrationDto();
        private bool _isPasswordVisible = false;
        private bool _isConfirmPasswordVisible = false;

        public async Task Register()
        {
            ShowRegistrationErrors = false;
            var result = await AuthenticationService.RegisterUser(_userForRegistration);
            if (!result.IsSuccessfulRegistration)
            {
                Errors = result.Errors;
                ShowRegistrationErrors = true;
            }
            else
            {
                NavigationManager.NavigateTo("/SuccessRegistration");
            }
        }

        private void TogglePassword()
        {
            _isPasswordVisible = !_isPasswordVisible;
            _isConfirmPasswordVisible = !_isConfirmPasswordVisible;
        }
    }
}
