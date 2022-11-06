using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smart.database.Migrations
{
    public partial class LogItemsHandlerName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HandlerName",
                table: "Log",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HandlerName",
                table: "Log");
        }
    }
}
