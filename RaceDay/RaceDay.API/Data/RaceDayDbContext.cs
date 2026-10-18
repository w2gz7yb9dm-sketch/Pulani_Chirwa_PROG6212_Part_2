
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;
using Route = RaceDay.API.Models.Route;

namespace RaceDay.API.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(DbContextOptions<RaceDayDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<RaceDay.API.Models.Route> Routes => Set<RaceDay.API.Models.Route>();
        public DbSet<Enrolment> Enrolments => Set<Enrolment>();
        public DbSet<Result> Results => Set<Result>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // An organiser can create multiple events.
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser)
                .WithMany(u => u.Events)
               .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Each event can have multiple categories.
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Each event can have one route.
            modelBuilder.Entity<RaceDay.API.Models.Route>()
                .HasOne(r => r.Event)
                .WithOne(e => e.Route)
                .HasForeignKey<RaceDay.API.Models.Route>(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // A participant can have multiple enrolments.
            modelBuilder.Entity<Enrolment>()
                .HasOne(en => en.Participant)
                .WithMany(u => u.Enrolments)
                .HasForeignKey(en => en.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);

            // An event can have multiple enrolments.
            modelBuilder.Entity<Enrolment>()
                .HasOne(en => en.Event)
                .WithMany(e => e.Enrolments)
                .HasForeignKey(en => en.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Each enrolment belongs to a category.
            modelBuilder.Entity<Enrolment>()
                .HasOne(en => en.Category)
                .WithMany(c => c.Enrolments)
                .HasForeignKey(en => en.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Each enrolment can have one result.
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrolment)
                .WithOne(en => en.Result)
                .HasForeignKey<Result>(r => r.EnrolmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // A user can capture multiple results.
            modelBuilder.Entity<Result>()
                .HasOne(r => r.CapturedByUser)
                .WithMany(u => u.CapturedResults)
                .HasForeignKey(r => r.CapturedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prevent duplicate email addresses.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Prevent duplicate enrolments for the same event.
            modelBuilder.Entity<Enrolment>()
                .HasIndex(en => new { en.ParticipantId, en.EventId })
                .IsUnique();

            // Prevent duplicate category names within an event.
            modelBuilder.Entity<Category>()
                .HasIndex(c => new { c.EventId, c.Name })
                .IsUnique();
        }
    }
}
