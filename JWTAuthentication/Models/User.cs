using System.ComponentModel.DataAnnotations;

namespace JWTAuthentication.Models
{
    public class User
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }

        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
