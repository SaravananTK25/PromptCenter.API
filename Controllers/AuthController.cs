using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PromptCenter.API.Data;
using PromptCenter.API.models;
using PromptCenter.API.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PromptCenter.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public AuthController(
            AppDbContext context,
            IConfiguration configuration,
            IWebHostEnvironment environment)
        {
            _context = context;
            _passwordHasher = new PasswordHasher<User>();
            _configuration = configuration;
            _environment = environment;
        }       

        [HttpPost("register")]
        public IActionResult Register(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new
                {
                    message = "Name, email and password are required."
                });
            }

            var existingUser = _context.Users
                .FirstOrDefault(u => u.Email == request.Email);

            if (existingUser != null)
            {
                return BadRequest(new
                {
                    message = "Email is already registered."
                });
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                IsAdmin = false
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                request.Password
            );

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new
            {
                message = "Registration successful."
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            var claims = new[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                ),

                new Claim(
                    ClaimTypes.Name,
                    user.Name
                ),

                new Claim(
                    ClaimTypes.Email,
                    user.Email
                ),

                new Claim(
                    ClaimTypes.Role,
                    user.IsAdmin ? "Admin" : "User"
                )
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler()
                    .WriteToken(token),
                name = user.Name,
                email = user.Email,
                role = user.IsAdmin ? "Admin" : "User"
            });
        }

        // =========================
        // FORGOT PASSWORD
        // =========================

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new
                {
                    message = "Email is required."
                });
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            // Do not reveal whether the email exists.
            if (user == null)
            {
                return Ok(new
                {
                    message =
                        "If an account exists with this email, password reset instructions have been generated."
                });
            }

            // Generate a secure random token
            var randomBytes = RandomNumberGenerator.GetBytes(32);

            var resetToken = Convert
                .ToBase64String(randomBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            user.PasswordResetToken = resetToken;

            // Token expires after 15 minutes
            user.PasswordResetTokenExpiresAt =
                DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();

            // Development only.
            // Later we will send this through email.
            if (_environment.IsDevelopment())
            {
            }

            return Ok(new
            {
                message =
                    "If an account exists with this email, password reset instructions have been generated.",
                  resetToken
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
                return BadRequest(new { message = "Invalid reset token." });

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest(new { message = "Password is required." });

            if (request.NewPassword.Length < 6)
                return BadRequest(new { message = "Password must be at least 6 characters." });

            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.PasswordResetToken == request.Token &&
                    u.PasswordResetTokenExpiresAt > DateTime.UtcNow);

            if (user == null)
                return BadRequest(new
                {
                    message = "This reset link is invalid or has expired."
                });

            var passwordHasher = new PasswordHasher<User>();

            user.PasswordHash = passwordHasher.HashPassword(
                user,
                request.NewPassword
            );

            // Token can only be used once
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiresAt = null;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Password reset successfully."
            });
        }
      
    }
}