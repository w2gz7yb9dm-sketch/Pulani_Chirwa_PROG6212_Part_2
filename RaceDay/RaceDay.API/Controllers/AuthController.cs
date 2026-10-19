
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        private readonly SessionUserService _session;

        public AuthController(
            RaceDayDbContext db,
            SessionUserService session)
        {
            _db = db;
            _session = session;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            RegisterRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            if (await _db.Users.AnyAsync(
                u => u.Email.ToLower() == email))
                return Conflict(new
                {
                    message = "Email already registered."
                });

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    request.Password),
                PhoneNumber = request.PhoneNumber,
                DateOfBirth = request.DateOfBirth,
                Role = "Participant",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _db.Users.Add(user);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Unable to register account."
                });
            }

            return StatusCode(201, new
            {
                user.UserId,
                user.Email,
                user.Role
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequest request)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            var user = await _db.Users.FirstOrDefaultAsync(
                u => u.Email.ToLower() == email && u.IsActive);

            bool valid = false;

            if (user != null)
            {
                try
                {
                    valid = BCrypt.Net.BCrypt.Verify(
                        request.Password, user.PasswordHash);
                }
                catch (Exception ex) when (
                    ex is ArgumentException ||
                    ex is FormatException)
                {
                    valid = false;
                }
            }

            if (!valid || user == null)
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });

            HttpContext.Session.Clear();
            HttpContext.Session.SetInt32(
                "UserId", user.UserId);

            return Ok(new
            {
                message = "Login successful.",
                user.UserId,
                user.Email,
                user.Role
            });
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var user = await _session.GetUserAsync();

            if (user == null)
                return Unauthorized(new
                {
                    message = "Please log in."
                });

            return Ok(new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return Ok(new
            {
                message = "Logged out successfully."
            });
        }
    }
}
