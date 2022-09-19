using Microsoft.EntityFrameworkCore.Migrations;
using Reservation.Postgres;
using Reservation.Settings.Infrastructure;

#nullable disable

namespace Reservation.Settings.Infrastructure.Migrations
{
    public partial class Add_NewColumn_RowsAndColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<SessionSpots[]>(
                name: "RowsAndColumns",
                table: "Sessions",
                type: "jsonb",
                nullable: false,
                defaultValue: new SessionSpots[0]);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowsAndColumns",
                table: "Sessions");
        }
    }
}
