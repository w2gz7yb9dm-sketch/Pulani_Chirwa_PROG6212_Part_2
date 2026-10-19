
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.Services;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _db;
        private readonly SessionUserService _session;

        public CategoriesController(
            RaceDayDbContext db,
            SessionUserService session)
        {
            _db = db;
            _session = session;
        }

        public class CreateCategoryRequest
        {
            [Range(1, int.MaxValue)]
            public int EventId { get; set; }

            [Required, MaxLength(100)]
            public string Name { get; set; } = "";

            [Range(typeof(decimal), "0.01", "9999.99")]
            public decimal DistanceKm { get; set; }

            [Range(typeof(decimal), "0", "99999999.99")]
            public decimal EntryFee { get; set; }

            public byte? MinimumAge { get; set; }

            [Range(1, int.MaxValue)]
            public int Capacity { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _db.Categories
                .AsNoTracking()
                .Select(c => new {
                    c.CategoryId,
                    c.EventId,
                    c.Name,
                    c.DistanceKm,
                    c.EntryFee,
                    c.MinimumAge,
                    c.Capacity
                }).ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _db.Categories
                .AsNoTracking()
                .Where(c => c.CategoryId == id)
                .Select(c => new {
                    c.CategoryId,
                    c.EventId,
                    c.Name,
                    c.DistanceKm,
                    c.EntryFee,
                    c.MinimumAge,
                    c.Capacity
                }).FirstOrDefaultAsync();

            return category == null
                ? NotFound() : Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateCategoryRequest request)
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

            if (await _db.Categories.AnyAsync(c =>
                c.EventId == request.EventId &&
                c.Name == request.Name))
                return Conflict(new
                {
                    message = "Category already exists."
                });

            var category = new Category
            {
                EventId = request.EventId,
                Name = request.Name.Trim(),
                DistanceKm = request.DistanceKm,
                EntryFee = request.EntryFee,
                MinimumAge = request.MinimumAge,
                Capacity = request.Capacity
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetById),
                new { id = category.CategoryId },
                new { category.CategoryId });
        }
    }
}
