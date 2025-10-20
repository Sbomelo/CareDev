using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CareDev.Models
{
    public class User : IdentityUser
    {
        

        public string Name { get; set; }

        public string Email { get; set; }

        public string? pictureUrl { get; set; } 
    }
}
