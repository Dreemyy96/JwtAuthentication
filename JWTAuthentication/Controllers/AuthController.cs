using JWTAuthentication.Infrastructures;
using JWTAuthentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JWTAuthentication.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Registration([FromBody] RegisterUser user)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }

            try
            {
                await _authService.Register(user);

                return Ok();
            }
            catch (BadHttpRequestException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
        }
 
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return Unauthorized();
            }

            try
            {
                string token = await _authService.Login(loginRequest.Username, loginRequest.Password);

                Response.Cookies.Append("user-token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiryInMinutes"]))
                });

                return Ok();
            }
            catch (BadHttpRequestException ex)
            {
                return Unauthorized(new {message = ex.Message});
            }
        }
    }
}
