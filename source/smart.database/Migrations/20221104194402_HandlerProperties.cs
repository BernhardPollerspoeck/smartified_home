using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smart.database.Migrations
{
    public partial class HandlerProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProcessRunning",
                table: "ElementHandlers");

            migrationBuilder.RenameColumn(
                name: "SignalConnected",
                table: "ElementHandlers",
                newName: "Connected");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Connected",
                table: "ElementHandlers",
                newName: "SignalConnected");

            migrationBuilder.AddColumn<bool>(
                name: "ProcessRunning",
                table: "ElementHandlers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
