using Objects.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Dto.Authentication
{
    public class UserForRegistrationDto
    {
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;
        [Required(ErrorMessage = "Native language is required")]
        public string NativeLanguage { get; set; } = null!;
        [Required(ErrorMessage = "Language to learn is required")]
        public string DesiredLanguage { get; set; } = null!;
    }

    public class RegistrationResponseDto
    {
        public bool IsSuccessfulRegistration { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
