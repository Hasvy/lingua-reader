using Objects.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<RegistrationResponseDto> RegisterUser(UserForRegistrationDto userForRegistration);
        Task<AuthResponseDto> Login(UserForAuthenticationDto userForAuthentication);
        Task Logout();
        Task SendEmail(ForgotPasswordDto forgotPasswordDto);
        Task ResetPassword(ResetPasswordDto resetPasswordDto);
        //Task<string> RefreshToken();
        Task<ConfirmEmailResponseDto> ConfirmEmail(ConfirmEmailDto confirmEmailDto);
        Task<ConfirmEmailResponseDto> ResendConfirmationEmail(string email);
    }
}
