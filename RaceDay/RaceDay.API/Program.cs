
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;

namespace RaceDay.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register the RaceDay SQL Server database.
            builder.Services.AddDbContext<RaceDayDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString(
                        "DefaultConnection")));

            // Register API controllers.
            builder.Services.AddControllers();

            // Register OpenAPI services.
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Enable OpenAPI in development.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
