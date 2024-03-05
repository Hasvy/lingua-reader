using EmailService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Objects.Dto.Authentication;
using Objects.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace BlazorServer.Controllers
{
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _jwtSettings;
        private readonly IEmailSender _emailSender;

        public AccountsController(UserManager<ApplicationUser> userManager, IConfiguration configuration, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _configuration = configuration;
            _jwtSettings = _configuration.GetSection("JwtSettings");
        }

        [HttpPost]
        [Route("api/accounts/Registration")]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration == null || !ModelState.IsValid)
                return BadRequest();
            var user = new ApplicationUser { UserName = "user_" + Guid.NewGuid().ToString(),
                                             Email = userForRegistration.Email,
                                             NativeLanguage = userForRegistration.NativeLanguage,
                                             DesiredLanguage = userForRegistration.DesiredLanguage };
            var result = await _userManager.CreateAsync(user, userForRegistration.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new RegistrationResponseDto { Errors = errors });
            }
            await SendAddressConfirmationEmail(user);
            return StatusCode(201);
        }

        [HttpPost]
        [Route("api/accounts/Login")]
        public async Task<IActionResult> Login([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            var user = await _userManager.FindByEmailAsync(userForAuthentication.Email);

            if (user is not null)
            {
                bool isLockedOut = await _userManager.IsLockedOutAsync(user);
                if (isLockedOut)
                    return Unauthorized(new AuthResponseDto { ErrorMessage = "Your account is locked out for 5 minutes, because of many invalid login attempts. Please try again later." });
            }

            if (user is null || !await _userManager.CheckPasswordAsync(user, userForAuthentication.Password))
            {
                if (user is not null)
                {
                    await _userManager.AccessFailedAsync(user);
                }

                return Unauthorized(new AuthResponseDto { ErrorMessage = "Invalid Authentication" });
            }

            bool isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (!isEmailConfirmed)
                return Unauthorized(GetErrorWithResendLink(user, "The Email is not confirmed. Please confirm your email using a link, that is sent to your email address."));

            var signingCredentials = GetSigningCredentials();
            var claims = GetClaims(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
            var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            await _userManager.ResetAccessFailedCountAsync(user);
            return Ok(new AuthResponseDto { IsAuthSuccessful = true, Token = token });
        }

        [HttpPost]
        [Route("api/accounts/ForgotPassword")]
        public async Task<IActionResult> SendEmail([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user is null)
            {
                return Ok();        //For security reasons, hide info if user exists
            }
            
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var callback = $"{Request.Scheme}://{_configuration["ClientAddress"]}/ResetPassword?token={encodedToken}&email={Uri.EscapeDataString(user.Email)}";

            var message = new Message(new string[] { user.Email }, "Password reset link", callback);    //TODO check user emails
            await _emailSender.SendEmailAsync(message);

            return Ok();
        }

        [HttpPost]
        [Route("api/accounts/ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user is null)
            {
                return Ok();        //For security reasons
            }
            var resetPassResult = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description);

                return BadRequest(new ResetPasswordResponseDto { Errors = errors });
            }

            return Ok();
        }

        [HttpPost]
        [Route("api/accounts/ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirmEmailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email);
            if (user is null)
            {
                return Ok();        //For security reasons
            }
            var result = await _userManager.ConfirmEmailAsync(user, confirmEmailDto.Token);
            if (!result.Succeeded)
            {
                return BadRequest(GetErrorWithResendLink(user, "The token is not valid, probably it was expired."));
            }
            return Ok();
        }

        [HttpPost]
        [Route("api/accounts/ResendAddressConfirmationEmail")]
        public async Task<IActionResult> ResendAddressConfirmationEmail([FromBody] string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                return Ok();        //For security reasons
            }
            if (await _userManager.IsEmailConfirmedAsync(user))
            {
                return BadRequest( new ConfirmEmailResponseDto { ErrorMessage = "Email has been already confirmed, you can log in." });
            }
            await SendAddressConfirmationEmail(user);
            return Ok();
        }

        private async Task SendAddressConfirmationEmail(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = HttpUtility.UrlEncode(token);
            var confirmationLink = $"{Request.Scheme}://{_configuration["ClientAddress"]}/ConfirmEmail?token={encodedToken}&email={Uri.EscapeDataString(user.Email)}";
            var message = new Message(new string[] { user.Email }, "Confirmation email link", confirmationLink);
            await _emailSender.SendEmailAsync(message);
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(_jwtSettings["securityKey"]);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        private List<Claim> GetClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim("UserID", user.Id)
            };

            return claims;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var tokenOptions = new JwtSecurityToken(
                issuer: _jwtSettings["validIssuer"],
                audience: _jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettings["expiryInMinutes"])),
                signingCredentials: signingCredentials);

            return tokenOptions;
        }

        private ConfirmEmailResponseDto GetErrorWithResendLink(ApplicationUser user, string errorMessage)
        {
            return new ConfirmEmailResponseDto() { ErrorMessage = errorMessage,
                                                   UrlText = "Click here to resend the confirmation link to your email.",
                                                   Url = $"{Request.Scheme}://{_configuration["ClientAddress"]}/ResendConfirmationEmail?email={Uri.EscapeDataString(user.Email)}" };
        }
    }
}
