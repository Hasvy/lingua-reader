using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Dto
{
    public class UserProfileSettingsDto
    {
        //[Required(ErrorMessage = "Email is required.")]
        //public string Email { get; set; } = null!;
        //[Required(ErrorMessage = "Current password is required")]
        //public string CurrentPassword { get; set; } = null!;
        //[Required(ErrorMessage = "New Password is required")]
        //public string NewPassword { get; set; } = null!;
        //[Compare("Password", ErrorMessage = "The new password and confirmation password do not match.")]
        //public string ConfirmNewPassword { get; set; } = null!;
        [Required(ErrorMessage = "Language is required")]
        public string UserMainLang { get; set; } = null!;
    }

    public class UserProfileResponseDto
    {
        public bool IsSuccessfulChange { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }
}
