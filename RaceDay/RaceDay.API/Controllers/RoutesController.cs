
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.Services;
using System.ComponentModel.DataAnnotations;
using RaceRoute = RaceDay.API.Models.Route;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/routes")]
    public class RoutesController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        private readonly SessionUserService _session;

        public RoutesController(
            RaceDayDbContext db,
            SessionUserService session)
        {
            _db = db;
            _session = session;
        }

        public class RouteRequest
        {
            [Range(1, int.MaxValue)]
            public int EventId { get; set; }

            [Required, MaxLength(150)]
            public string RouteName { get; set; } = "";

            [Range(typeof(decimal), "-90", "90")]
            public decimal StartLatitude { get; set; }

            [Range(typeof(decimal), "-180", "180")]
            public decimal StartLongitude { get; set; }

            [MaxLength(500)]
            public string? MapUrl { get; set; }

            public int? ElevationGainM { get; set; }

            [MaxLength(1000)]
            public string? RouteNotes { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _db.Routes.AsNoTracking()
                .Select(r => new {
                    r.RouteId,
                    r.EventId,
                    r.RouteName,
                    r.StartLatitude,
                    r.StartLongitude,
                    r.MapUrl,
                    r.ElevationGainM,
                    r.RouteNotes
                }).ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var route = await _db.Routes.AsNoTracking()
                .Where(r => r.RouteId == id)
                .Select(r => new {
                    r.RouteId,
                    r.EventId,
                    r.RouteName,
                    r.StartLatitude,
                    r.StartLongitude,
                    r.MapUrl,
                    r.ElevationGainM,
                    r.RouteNotes
                }).FirstOrDefaultAsync();

            return route == null ? NotFound() : Ok(route);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            RouteRequest request)
        {
            var user = await _session.GetUserAsync();
            if (user == null) return Unauthorized();

            var raceEvent = await _db.Events.FindAsync(
                request.EventId);

            if (raceEvent == null) return NotFound(
                new { message = "Event not found." });

            if (raceEvent.OrganiserId != user.UserId &&
                !await _session.IsAdminAsync())
                return StatusCode(403,
                    new { message = "Not your event." });

            if (await _db.Routes.AnyAsync(
                r => r.EventId == request.EventId))
                return Conflict(new
                {
                    message = "This event already has a route."
                });

            var route = new RaceRoute
            {
                EventId = request.EventId,
                RouteName = request.RouteName.Trim(),
                StartLatitude = request.StartLatitude,
                StartLongitude = request.StartLongitude,
                MapUrl = request.MapUrl,
                ElevationGainM = request.ElevationGainM,
                RouteNotes = request.RouteNotes
            };

            _db.Routes.Add(route);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = route.RouteId },
                new { route.RouteId });
        }
    }
}
