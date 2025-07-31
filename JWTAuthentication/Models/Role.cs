using System.ComponentModel.DataAnnotations;
using JWTAuthentication.Data;

namespace JWTAuthentication.Models
{
    public class Role
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public Roles Name { get; set; }

        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
