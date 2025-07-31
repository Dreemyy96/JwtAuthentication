using JWTAuthentication.Data;
using JWTAuthentication.Models;
using Microsoft.Identity.Client;

namespace JWTAuthentication.Infrastructures
{
    public interface IAuthService
    {
        public Task Register(RegisterUser user, Roles userRole = Roles.User);
        public Task<string> Login(string username, string password);
    }
}
