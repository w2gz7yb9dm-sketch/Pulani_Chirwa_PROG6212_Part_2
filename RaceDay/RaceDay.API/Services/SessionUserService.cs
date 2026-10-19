
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;

namespace RaceDay.API.Services
{
    public class SessionUserService
    {
        private readonly RaceDayDbContext _db;
        private readonly IHttpContextAccessor _accessor;

        public SessionUserService(
            RaceDayDbContext db,
            IHttpContextAccessor accessor)
        {
            _db = db;
            _accessor = accessor;
        }

        public int? UserId =>
            _accessor.HttpContext?.Session.GetInt32("UserId");

        public async Task<User?> GetUserAsync()
        {
            if (UserId == null) return null;

            return await _db.Users.FirstOrDefaultAsync(
                u => u.UserId == UserId && u.IsActive);
        }

        public async Task<bool> IsOrganiserAsync()
        {
            var user = await GetUserAsync();

            return user != null &&
                (user.Role.Equals("Organiser",
                    StringComparison.OrdinalIgnoreCase)
                 || user.Role.Equals("Admin",
                    StringComparison.OrdinalIgnoreCase));
        }

        public async Task<bool> IsAdminAsync()
        {
            var user = await GetUserAsync();

            return user != null &&
                user.Role.Equals("Admin",
                    StringComparison.OrdinalIgnoreCase);
        }
    }
}
