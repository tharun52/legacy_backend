using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BlogApi.Data;
using BlogApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BlogApi.Controllers
{
    public record AuthRequest(string Username, string Password);

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly BlogContext _context;
        private readonly IConfiguration _config;

        public AuthController(BlogContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(AuthRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest(new { message = "Username and password are required" });

            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return BadRequest(new { message = "Username already taken" });

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Registered successfully" });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid username or password" });

            var token = GenerateToken(user);
            return Ok(new { token, username = user.Username });
        }

        private string GenerateToken(User user)
        {
            var jwtKey = _config["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),  // reduced from 7 days to 1 hour
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
