using JWTAuthentication.Data;
using JWTAuthentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthentication.Infrastructures
{
    public class AuthServcie : IAuthService
    {
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthServcie(IPasswordHasher<User> passwordHasher, 
            ApplicationDbContext context,
            JwtService jwtService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _jwtService = jwtService;
        }
        public async Task Register(RegisterUser user)
        {
            if (await _context.Users.AnyAsync(u=>u.Username == user.Username))
            {
                throw new BadHttpRequestException("Пользователь с таким именем уже существует");
            }
            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                throw new BadHttpRequestException("Пользователь с такой почтой уже существует");
            }

            User newUser = new User { Username = user.Username, Email = user.Email };
            string passwordHash = _passwordHasher.HashPassword(newUser, user.Password);
            newUser.PasswordHash = passwordHash;

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == Roles.User);

            if(role == null)
            {
                throw new BadHttpRequestException("Роль не найдена");
            }
            _context.UserRoles.Add(new UserRoles { UserId = newUser.Id, RoleId = role.Id });

            await _context.SaveChangesAsync();
        }

        public async Task<string> Login(string username, string password)
        {
            var user  = await _context.Users.FirstOrDefaultAsync(u=>u.Username ==  username);

            if(user == null || !VerifyPassword(user, password)) 
            {
                throw new BadHttpRequestException("Неверный логин ипи пароль", StatusCodes.Status401Unauthorized);
            }

            UserRoles userRole = await _context.UserRoles.Include(ur=>ur.Role).FirstOrDefaultAsync(ur => ur.UserId == user.Id);

            if(userRole?.Role == null)
            {
                throw new BadHttpRequestException("Учетная запись не содержит роли");
            }

            string roleName = _context.Roles.FirstOrDefault(r => r.Id == userRole.RoleId).Name.ToString();

            string token = _jwtService.GetJwtToken(user.Username, roleName);

            return token;
        }

        private bool VerifyPassword(User user, string password)
        {
            var res = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return res == PasswordVerificationResult.Success;
        }
    }
}
