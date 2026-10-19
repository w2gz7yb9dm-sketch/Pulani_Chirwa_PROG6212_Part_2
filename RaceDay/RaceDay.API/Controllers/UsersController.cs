
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Services;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        private readonly SessionUserService _session;

        public UsersController(
            RaceDayDbContext db,
            SessionUserService session)
        {
            _db = db;
            _session = session;
        }

        public class UpdateProfileRequest
        {
            [Required, MaxLength(50)]
            public string FirstName { get; set; } = "";

            [Required, MaxLength(50)]
            public string LastName { get; set; } = "";

            [MaxLength(20)]
            public string? PhoneNumber { get; set; }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _session.GetUserAsync();
            if (user == null) return Unauthorized();

            return Ok(new
            {
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PhoneNumber,
                user.Role,
                user.DateOfBirth
            });
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateProfile(
            UpdateProfileRequest request)
        {
            var user = await _session.GetUserAsync();
            if (user == null) return Unauthorized();

            user.FirstName = request.FirstName.Trim();
            user.LastName = request.LastName.Trim();
            user.PhoneNumber = request.PhoneNumber;

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Profile updated successfully."
            });
        }
    }
}
