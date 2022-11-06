using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smart.database.Migrations
{
    public partial class HomeElementInHandler : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImageInfos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ImageInfos",
                columns: table => new
                {
                    ElementType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Tag = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageInfos", x => x.ElementType);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
