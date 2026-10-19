
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Services;

namespace RaceDay.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ==========================================
            // 1. DATABASE CONNECTION
            // ==========================================

            builder.Services.AddDbContext<RaceDayDbContext>(
                options =>
                    options.UseSqlServer(
                        builder.Configuration.GetConnectionString(
                            "DefaultConnection")));

            // ==========================================
            // 2. API CONTROLLERS
            // ==========================================

            builder.Services.AddControllers();

            // ==========================================
            // 3. SWAGGER / OPENAPI
            // ==========================================

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ==========================================
            // 4. SESSION CONFIGURATION
            // ==========================================

            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout =
                    TimeSpan.FromMinutes(30);

                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite =
                    SameSiteMode.Lax;

                options.Cookie.SecurePolicy =
                    CookieSecurePolicy.Always;
            });

            // ==========================================
            // 5. REGISTER APPLICATION SERVICES
            // ==========================================

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddScoped<SessionUserService>();

            // ==========================================
            // 6. BUILD APPLICATION
            // ==========================================

            var app = builder.Build();

            // ==========================================
            // 7. SWAGGER IN DEVELOPMENT
            // ==========================================

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // ==========================================
            // 8. HTTP REQUEST PIPELINE
            // ==========================================

            //app.UseHttpsRedirection();

            app.UseRouting();

            // Enable session storage.
            app.UseSession();

            app.UseAuthorization();

            // Enable API controller endpoints.
            app.MapControllers();

            // ==========================================
            // 9. RUN APPLICATION
            // ==========================================

            app.Run();
        }
    }
}
