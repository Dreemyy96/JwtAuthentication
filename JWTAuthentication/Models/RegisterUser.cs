using System.ComponentModel.DataAnnotations;

namespace JWTAuthentication.Models
{
    public class RegisterUser
    {
        [Required]
        [StringLength(25, MinimumLength = 3)]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
