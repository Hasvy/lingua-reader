using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Dto
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        public string Email { get; set; }
        public string Token { get; set; }
    }
}
