using Microsoft.AspNetCore.Components;
using Objects.Dto.Authentication;
using Services.Authentication;

namespace BlazorApp.Pages.Authentication
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
