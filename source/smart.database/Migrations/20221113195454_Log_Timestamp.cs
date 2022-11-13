using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smart.database.Migrations
{
    public partial class Log_Timestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "ElementHandlers");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "Log",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Log");

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "ElementHandlers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
