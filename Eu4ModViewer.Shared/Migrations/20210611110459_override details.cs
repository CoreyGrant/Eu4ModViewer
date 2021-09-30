using Microsoft.EntityFrameworkCore.Migrations;

namespace Eu4ModViewer.Shared.Migrations
{
    public partial class overridedetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OverrideDescription",
                table: "ModQueue",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OverrideTitle",
                table: "ModQueue",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverrideDescription",
                table: "ModQueue");

            migrationBuilder.DropColumn(
                name: "OverrideTitle",
                table: "ModQueue");
        }
    }
}
