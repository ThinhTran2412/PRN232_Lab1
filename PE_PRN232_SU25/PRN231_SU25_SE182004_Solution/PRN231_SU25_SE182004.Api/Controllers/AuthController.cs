using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PRN231_SU25_SE182004.Repositories.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Text;

namespace PRN231_SU25_SE182004.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SU25LeopardDBContext _context;
        private readonly IConfiguration _config;

        public AuthController(SU25LeopardDBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _context.LeopardAccounts.FirstOrDefault(x => x.Email == request.Email && x.Password == request.Password);

            if (user == null) return Unauthorized(new {errorCode = "HB40101", message = "Sai tài khoản hoặc mật khẩu"});

            if(user.RoleId != 4 && user.RoleId != 5 && user.RoleId != 6 && user.RoleId != 7)
                return Unauthorized(new { errorCode = "HB40301", message = "Role không được cấp phép" });

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                }),

                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return Ok(new { token = tokenHandler.WriteToken(token), role = user.RoleId.ToString() });
        }

    }
}

public class LoginRequest { public string Email { get; set; } public string Password { get; set; } }