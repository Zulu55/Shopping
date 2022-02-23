using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shooping.Migrations
{
    public partial class AddIndexToCountry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Countries_Name",
                table: "Countries",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Countries_Name",
                table: "Countries");
        }
    }
}
