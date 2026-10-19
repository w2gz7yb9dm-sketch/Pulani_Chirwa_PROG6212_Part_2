
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Controllers;
using RaceDay.API.Data;
using RaceDay.API.Models;
using RaceDay.API.Services;
using Xunit;

namespace RaceDay.Tests
{
    public class UnitTest1
    {
        // Create a separate in-memory database for each test.
        private static RaceDayDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<RaceDayDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new RaceDayDbContext(options);
        }

        // Create EventsController with both required services.
        private static EventsController CreateController(
            RaceDayDbContext context)
        {
            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext()
            };

            var sessionService = new SessionUserService(
                context,
                httpContextAccessor
            );

            return new EventsController(
                context,
                sessionService
            );
        }

        [Fact]
        public async Task GetEvents_WhenDatabaseIsEmpty_ReturnsEmptyList()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            // Act
            var result = await controller.GetEvents();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);

            var json = System.Text.Json.JsonSerializer.Serialize(
                okResult.Value
            );

            Assert.Equal("[]", json);
        }

        [Fact]
        public async Task GetEvent_WhenEventDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            using var context = CreateContext();
            var controller = CreateController(context);

            // Act
            var result = await controller.GetEvent(999);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetEvent_WhenEventExists_ReturnsOk()
        {
            // Arrange
            using var context = CreateContext();

            context.Events.Add(new Event
            {
                EventId = 1,
                OrganiserId = 1,
                Name = "RaceDay Test Run",
                Description = "Automated testing event",
                EventType = "Running",
                DistanceKm = 5,
                StartDateTime = new DateTime(2027, 5, 10, 8, 0, 0),
                VenueName = "Test Stadium",
                City = "Johannesburg",
                Province = "Gauteng",
                RegistrationCloseDate =
                    new DateTime(2027, 5, 5, 23, 59, 0),
                Status = "Published"
            });

            await context.SaveChangesAsync();

            var controller = CreateController(context);

            // Act
            var result = await controller.GetEvent(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
