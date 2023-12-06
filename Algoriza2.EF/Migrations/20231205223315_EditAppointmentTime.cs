using Microsoft.EntityFrameworkCore.Migrations;

namespace Algoriza2.EF.Migrations
{
    public partial class EditAppointmentTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "Appointments");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "AppointmentTimes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "AppointmentTimes");

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
