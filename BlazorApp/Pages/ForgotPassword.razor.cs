using Microsoft.AspNetCore.Components;
using Objects.Dto;
using Services.Authentication;

namespace BlazorApp.Pages
{
    public partial class ForgotPassword : ComponentBase
    {
        private ForgotPasswordDto _forgotPasswordDto = new ForgotPasswordDto();
        [Inject] public IAuthenticationService AuthenticationService { get; set; } = null!;

        public async Task SendEmail()
        {
            await AuthenticationService.SendEmail(_forgotPasswordDto);
        }
    }
}
