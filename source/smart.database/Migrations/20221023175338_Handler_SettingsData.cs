using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smart.database.Migrations
{
    public partial class Handler_SettingsData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SettingsData",
                table: "ElementHandlers",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ElementHandlers",
                keyColumn: "SettingsData",
                keyValue: null,
                column: "SettingsData",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "SettingsData",
                table: "ElementHandlers",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
