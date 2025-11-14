using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace secondhand_marketplace.Migrations
{
    /// <inheritdoc />
    public partial class AddImagePathToListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Listings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "Description", "ImagePath", "Title" },
                values: new object[] { "GEN", "Perfect condition, works just as new", "/images/listings/lamp.jpg", "Desk Lamp" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Listings");

            migrationBuilder.UpdateData(
                table: "Listings",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "Description", "Title" },
                values: new object[] { "BOOK", "Good condition, highlights in first 2 chapters", "Discrete Math Textbook" });
        }
    }
}
