
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RaceDay.API.Migrations
{
    public partial class InitialBaseline : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Baseline migration.
            // The six RaceDay database tables already exist.
            // Do not recreate them or modify existing data.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Do not delete existing tables when rolling back
            // the baseline migration.
        }
    }
}
