using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Objects.Dto.Authentication
{
    public class ConfirmEmailDto
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
    }

    public class ConfirmEmailResponseDto
    {
        public bool IsSuccessfulConfirmation { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Url { get; set; }
        public string? UrlText { get; set; }
    }
}
