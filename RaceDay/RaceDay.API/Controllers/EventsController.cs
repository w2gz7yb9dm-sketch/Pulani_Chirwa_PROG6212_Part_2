
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.Services;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;
        private readonly SessionUserService _session;

        public EventsController(
            RaceDayDbContext context,
            SessionUserService session)
        {
            _context = context;
            _session = session;
        }

        // ==========================================
        // REQUEST MODEL
        // ==========================================

        public class EventRequest
        {
            [Required]
            [MaxLength(200)]
            public string Name { get; set; } =
                string.Empty;

            public string? Description { get; set; }

            public string? EventType { get; set; }

            [Range(typeof(decimal), "0.01", "999999")]
            public decimal? DistanceKm { get; set; }

            [Required]
            public DateTime StartDateTime { get; set; }

            public string? VenueName { get; set; }

            public string? City { get; set; }

            public string? Province { get; set; }

            public DateTime? RegistrationCloseDate
            {
                get;
                set;
            }
        }

        // ==========================================
        // GET ALL EVENTS
        // GET: api/events
        // ==========================================

        [HttpGet]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events
                .AsNoTracking()
                .Select(e => new
                {
                    e.EventId,
                    e.Name,
                    e.Description,
                    e.EventType,
                    e.DistanceKm,
                    e.StartDateTime,
                    e.VenueName,
                    e.City,
                    e.Province,
                    e.RegistrationCloseDate,
                    e.Status,
                    e.OrganiserId
                })
                .ToListAsync();

            return Ok(events);
        }

        // ==========================================
        // GET EVENT BY ID
        // GET: api/events/1
        // ==========================================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEvent(
            int id)
        {
            var raceEvent = await _context.Events
                .AsNoTracking()
                .Where(e => e.EventId == id)
                .Select(e => new
                {
                    e.EventId,
                    e.Name,
                    e.Description,
                    e.EventType,
                    e.DistanceKm,
                    e.StartDateTime,
                    e.VenueName,
                    e.City,
                    e.Province,
                    e.RegistrationCloseDate,
                    e.Status,
                    e.OrganiserId
                })
                .FirstOrDefaultAsync();

            if (raceEvent == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            return Ok(raceEvent);
        }

        // ==========================================
        // CREATE EVENT
        // POST: api/events
        // ==========================================

        [HttpPost]
        public async Task<IActionResult> CreateEvent(
            [FromBody] EventRequest request)
        {
            var user = await _session.GetUserAsync();

            if (user == null)
            {
                return Unauthorized(new
                {
                    message =
                        "Please log in to create an event."
                });
            }

            if (!await _session.IsOrganiserAsync())
            {
                return StatusCode(403, new
                {
                    message =
                        "Only organisers and administrators " +
                        "can create events."
                });
            }

            if (request.StartDateTime <= DateTime.UtcNow)
            {
                return BadRequest(new
                {
                    message =
                        "Event start date must be in the future."
                });
            }

            if (request.RegistrationCloseDate.HasValue &&
                request.RegistrationCloseDate.Value >=
                    request.StartDateTime)
            {
                return BadRequest(new
                {
                    message =
                        "Registration must close before " +
                        "the event starts."
                });
            }

            var raceEvent = new Event
            {
                Name = request.Name.Trim(),
                Description = request.Description,
                EventType = request.EventType,
                DistanceKm = request.DistanceKm,
                StartDateTime = request.StartDateTime,
                VenueName = request.VenueName,
                City = request.City,
                Province = request.Province,
                RegistrationCloseDate =
                    request.RegistrationCloseDate,

                OrganiserId = user.UserId,

                Status = "Draft",

                CreatedAt = DateTime.UtcNow
            };

            _context.Events.Add(raceEvent);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = raceEvent.EventId },
                new
                {
                    message =
                        "Event created successfully.",
                    raceEvent.EventId,
                    raceEvent.Name,
                    raceEvent.Status
                });
        }

        // ==========================================
        // UPDATE EVENT
        // PUT: api/events/1
        // ==========================================

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEvent(
            int id,
            [FromBody] EventRequest request)
        {
            var user = await _session.GetUserAsync();

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Please log in."
                });
            }

            var raceEvent =
                await _context.Events.FindAsync(id);

            if (raceEvent == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            bool isAdmin =
                await _session.IsAdminAsync();

            if (raceEvent.OrganiserId != user.UserId &&
                !isAdmin)
            {
                return StatusCode(403, new
                {
                    message =
                        "You can only update your own events."
                });
            }

            if (request.StartDateTime <= DateTime.UtcNow)
            {
                return BadRequest(new
                {
                    message =
                        "Event start date must be in the future."
                });
            }

            if (request.RegistrationCloseDate.HasValue &&
                request.RegistrationCloseDate.Value >=
                    request.StartDateTime)
            {
                return BadRequest(new
                {
                    message =
                        "Registration must close before " +
                        "the event starts."
                });
            }

            raceEvent.Name = request.Name.Trim();
            raceEvent.Description =
                request.Description;
            raceEvent.EventType =
                request.EventType;
            raceEvent.DistanceKm =
                request.DistanceKm;
            raceEvent.StartDateTime =
                request.StartDateTime;
            raceEvent.VenueName =
                request.VenueName;
            raceEvent.City = request.City;
            raceEvent.Province = request.Province;
            raceEvent.RegistrationCloseDate =
                request.RegistrationCloseDate;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message =
                    "Event updated successfully.",
                raceEvent.EventId,
                raceEvent.Name
            });
        }

        // ==========================================
        // DELETE EVENT
        // DELETE: api/events/1
        // ==========================================

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEvent(
            int id)
        {
            var user = await _session.GetUserAsync();

            if (user == null)
            {
                return Unauthorized(new
                {
                    message = "Please log in."
                });
            }

            var raceEvent =
                await _context.Events.FindAsync(id);

            if (raceEvent == null)
            {
                return NotFound(new
                {
                    message = "Event not found."
                });
            }

            bool isAdmin =
                await _session.IsAdminAsync();

            if (raceEvent.OrganiserId != user.UserId &&
                !isAdmin)
            {
                return StatusCode(403, new
                {
                    message =
                        "You can only delete your own events."
                });
            }

            // Do not delete events that have enrolments.
            // This protects participant records.
            bool hasEnrolments =
                await _context.Enrolments.AnyAsync(
                    e => e.EventId == id);

            if (hasEnrolments)
            {
                return Conflict(new
                {
                    message =
                        "This event has enrolments and " +
                        "cannot be deleted."
                });
            }

            _context.Events.Remove(raceEvent);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message =
                    "Event deleted successfully."
            });
        }
    }
}
