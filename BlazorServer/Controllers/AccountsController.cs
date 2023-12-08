using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V5.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;
using Objects.Entities;

namespace BlazorServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public AccountsController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            var newUser = new AppUser { UserName = "testuser1@example.com", Email = "testuser1@example.com" };

            var result = await _userManager.CreateAsync(newUser);

            if (result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);
                return NotFound();
            }
            return Ok();
        }
    }
}
