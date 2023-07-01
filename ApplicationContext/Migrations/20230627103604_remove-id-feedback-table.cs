using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApplicationContext.Migrations
{
    public partial class removeidfeedbacktable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Feedback_ReviewId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ReviewId",
                table: "Reservations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedback",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "ReviewId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Feedback");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReviewId",
                table: "Reservations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Feedback",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedback",
                table: "Feedback",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ReviewId",
                table: "Reservations",
                column: "ReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Feedback_ReviewId",
                table: "Reservations",
                column: "ReviewId",
                principalTable: "Feedback",
                principalColumn: "Id");
        }
    }
}
