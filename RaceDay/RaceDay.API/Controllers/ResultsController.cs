
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.Services;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/results")]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        private readonly SessionUserService _session;

        public ResultsController(
            RaceDayDbContext db,
            SessionUserService session)
        {
            _db = db;
            _session = session;
        }

        public class CaptureResultRequest
        {
            [Range(1, int.MaxValue)]
            public int EnrolmentId { get; set; }

            public TimeSpan? FinishTime { get; set; }

            [Range(1, int.MaxValue)]
            public int? OverallPosition { get; set; }

            [Range(1, int.MaxValue)]
            public int? CategoryPosition { get; set; }

            [Required, MaxLength(20)]
            public string ResultStatus { get; set; } =
                "Finished";
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _db.Results.AsNoTracking()
                .Select(r => new {
                    r.ResultId,
                    r.EnrolmentId,
                    r.FinishTime,
                    r.OverallPosition,
                    r.CategoryPosition,
                    r.ResultStatus
                }).ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _db.Results.AsNoTracking()
                .Where(r => r.ResultId == id)
                .Select(r => new {
                    r.ResultId,
                    r.EnrolmentId,
                    r.FinishTime,
                    r.OverallPosition,
                    r.CategoryPosition,
                    r.ResultStatus
                }).FirstOrDefaultAsync();

            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Capture(
            CaptureResultRequest request)
        {
            var user = await _session.GetUserAsync();
            if (user == null) return Unauthorized();

            var enrolment = await _db.Enrolments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e =>
                    e.EnrolmentId == request.EnrolmentId);

            if (enrolment == null || enrolment.Event == null)
                return NotFound(new
                {
                    message = "Enrolment not found."
                });

            if (enrolment.Event.OrganiserId != user.UserId &&
                !await _session.IsAdminAsync())
                return StatusCode(403, new
                {
                    message = "Not authorised to capture results."
                });

            if (await _db.Results.AnyAsync(r =>
                r.EnrolmentId == request.EnrolmentId))
                return Conflict(new
                {
                    message = "Result already exists."
                });

            var result = new Result
            {
                EnrolmentId = request.EnrolmentId,
                FinishTime = request.FinishTime,
                OverallPosition = request.OverallPosition,
                CategoryPosition = request.CategoryPosition,
                ResultStatus = request.ResultStatus,
                CapturedByUserId = user.UserId,
                CapturedAt = DateTime.UtcNow
            };

            _db.Results.Add(result);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.ResultId },
                new { result.ResultId });
        }
    }
}
