using Microsoft.AspNetCore.Components;
using Objects.Dto;
using Services.Authentication;

namespace BlazorApp.Pages
{
    public partial class Registration : ComponentBase
    {
        private UserForRegistrationDto _userForRegistration = new UserForRegistrationDto();
        [Inject] public IAuthenticationService AuthenticationService { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        public bool ShowRegistrationErrors { get; set; }
        public IEnumerable<string>? Errors { get; set; }
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
                NavigationManager.NavigateTo("/");
            }
        }
    }
}
