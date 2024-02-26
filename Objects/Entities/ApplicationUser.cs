using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Objects.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string NativeLanguage { get; set; } = null!;
        public string DesiredLanguage { get; set; } = null!;
    }
}
