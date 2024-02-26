using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Objects.Dto;
using Objects.Entities;

namespace BlazorServer.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("api/user/GetNativeLanguage")]
        public async Task<string> GetNativeLanguage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is not null)
                return user.NativeLanguage ?? string.Empty;
            else
                return string.Empty;
        }

        [HttpGet]
        [Route("api/user/GetDesiredLanguage")]
        public async Task<string> GetDesiredLanguage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user is not null)
                return user.DesiredLanguage ?? string.Empty;
            else
                return string.Empty;
        }

        [HttpPost]
        [Route("api/user/ChangeUserSettings")]
        public async Task<IActionResult> ChangeUserSettings([FromBody] UserProfileSettingsDto userProfileSettingsDto)
        {
            if (userProfileSettingsDto == null || !ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.GetUserAsync(User);
            if (user is not null)
            {
                user.NativeLanguage = userProfileSettingsDto.NativeLanguage;
                user.DesiredLanguage = userProfileSettingsDto.DesiredLanguage;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return Ok();
            }
            return BadRequest();
        }
    }
}
