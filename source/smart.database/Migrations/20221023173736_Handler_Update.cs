using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smart.database.Migrations
{
    public partial class Handler_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Elements_ElementHandler_ElementHandlerId",
                table: "Elements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ElementHandler",
                table: "ElementHandler");

            migrationBuilder.RenameTable(
                name: "ElementHandler",
                newName: "ElementHandlers");

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "ElementHandlers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ElementHandlers",
                table: "ElementHandlers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Elements_ElementHandlers_ElementHandlerId",
                table: "Elements",
                column: "ElementHandlerId",
                principalTable: "ElementHandlers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Elements_ElementHandlers_ElementHandlerId",
                table: "Elements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ElementHandlers",
                table: "ElementHandlers");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "ElementHandlers");

            migrationBuilder.RenameTable(
                name: "ElementHandlers",
                newName: "ElementHandler");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ElementHandler",
                table: "ElementHandler",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Elements_ElementHandler_ElementHandlerId",
                table: "Elements",
                column: "ElementHandlerId",
                principalTable: "ElementHandler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
