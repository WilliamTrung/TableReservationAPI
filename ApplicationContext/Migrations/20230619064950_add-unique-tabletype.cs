using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationContext.Migrations
{
    public partial class adduniquetabletype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_TableTypes_Seat_Private",
                table: "TableTypes",
                columns: new[] { "Seat", "Private" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TableTypes_Seat_Private",
                table: "TableTypes");
        }
    }
}
