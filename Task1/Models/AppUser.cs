using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Task1.Models
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }

        public bool IsAdmin { get; set; }
    }
}
