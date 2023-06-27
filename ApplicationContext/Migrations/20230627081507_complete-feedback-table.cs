using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationContext.Migrations
{
    public partial class completefeedbacktable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Reviews");

            migrationBuilder.AddColumn<int>(
                name: "FacilityRating",
                table: "Reviews",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FoodRating",
                table: "Reviews",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceRating",
                table: "Reviews",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UtilityRating",
                table: "Reviews",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacilityRating",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "FoodRating",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "ServiceRating",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "UtilityRating",
                table: "Reviews");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Reviews",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
