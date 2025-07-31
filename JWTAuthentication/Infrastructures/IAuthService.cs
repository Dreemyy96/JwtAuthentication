using JWTAuthentication.Models;
using Microsoft.Identity.Client;

namespace JWTAuthentication.Infrastructures
{
    public interface IAuthService
    {
        public Task Register(RegisterUser user);
        public Task<string> Login(string username, string password);
    }
}
