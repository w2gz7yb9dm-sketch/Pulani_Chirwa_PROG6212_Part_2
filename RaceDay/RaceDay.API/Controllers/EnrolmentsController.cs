
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.Services;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/enrolments")]
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        private readonly SessionUserService _session;

        public EnrolmentsController(
            RaceDayDbContext db,
            SessionUserService session)
        {
            _db = db;
            _session = session;
        }

        public class EnrolRequest
        {
            [Range(1, int.MaxValue)]
            public int EventId { get; set; }

            [Range(1, int.MaxValue)]
            public int CategoryId { get; set; }
        }

        [HttpGet("mine")]
        public async Task<IActionResult> Mine()
        {
            var user = await _session.GetUserAsync();
            if (user == null) return Unauthorized();

            var records = await _db.Enrolments
                .AsNoTracking()
                .Where(e => e.ParticipantId == user.UserId)
                .Select(e => new {
                    e.EnrolmentId,
                    e.EventId,
                    e.CategoryId,
                    e.RaceNumber,
                    e.EnrolmentStatus,
                    e.PaymentStatus,
                    e.EnrolledAt
                }).ToListAsync();

            return Ok(records);
        }

        [HttpPost]
        public async Task<IActionResult> Enrol(
            EnrolRequest request)
        {
            var user = await _session.GetUserAsync();
            if (user == null) return Unauthorized();

            var category = await _db.Categories
                .FirstOrDefaultAsync(c =>
                    c.CategoryId == request.CategoryId &&
                    c.EventId == request.EventId);

            if (category == null)
                return BadRequest(new
                {
                    message = "Invalid event or category."
                });

            var raceEvent = await _db.Events.FindAsync(
                request.EventId);

            if (raceEvent == null)
                return NotFound();

            if (raceEvent.RegistrationCloseDate.HasValue &&
                raceEvent.RegistrationCloseDate.Value <
                    DateTime.UtcNow)
                return BadRequest(new
                {
                    message = "Registration has closed."
                });

            if (await _db.Enrolments.AnyAsync(e =>
                e.ParticipantId == user.UserId &&
                e.EventId == request.EventId))
                return Conflict(new
                {
                    message = "Already enrolled in this event."
                });

            int count = await _db.Enrolments.CountAsync(e =>
                e.CategoryId == request.CategoryId &&
                e.EnrolmentStatus != "Cancelled");

            if (count >= category.Capacity)
                return Conflict(new
                {
                    message = "Category is full."
                });

            var enrolment = new Enrolment
            {
                ParticipantId = user.UserId,
                EventId = request.EventId,
                CategoryId = request.CategoryId,
                EnrolledAt = DateTime.UtcNow,
                PaymentStatus = "Pending",
                EnrolmentStatus = "Active"
            };

            _db.Enrolments.Add(enrolment);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return Conflict(new
                {
                    message = "Enrolment could not be saved."
                });
            }

            return StatusCode(201, new
            {
                enrolment.EnrolmentId,
                enrolment.EventId,
                enrolment.CategoryId,
                enrolment.EnrolmentStatus
            });
        }

        [HttpPut("{id:int}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            var user = await _session.GetUserAsync();
            if (user == null) return Unauthorized();

            var enrolment = await _db.Enrolments
                .FirstOrDefaultAsync(e =>
                    e.EnrolmentId == id &&
                    e.ParticipantId == user.UserId);

            if (enrolment == null) return NotFound();

            enrolment.EnrolmentStatus = "Cancelled";
            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = "Enrolment cancelled."
            });
        }
    }
}
