using Microsoft.EntityFrameworkCore.Migrations;

namespace Eu4ModViewer.Shared.Migrations
{
    public partial class existingfiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExcludeFiles",
                table: "ModQueue",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExcludeFiles",
                table: "ModQueue");
        }
    }
}
