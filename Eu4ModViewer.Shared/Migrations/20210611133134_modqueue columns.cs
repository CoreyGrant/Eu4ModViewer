using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Eu4ModViewer.Shared.Migrations
{
    public partial class modqueuecolumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OverrideTitle",
                table: "ModQueue",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "OverrideDescription",
                table: "ModQueue",
                newName: "Description");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdated",
                table: "ModQueue",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdated",
                table: "ModQueue");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "ModQueue",
                newName: "OverrideTitle");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "ModQueue",
                newName: "OverrideDescription");
        }
    }
}
