using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationContext.Migrations
{
    public partial class adjustinitvalues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TableTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Private",
                value: false);

            migrationBuilder.UpdateData(
                table: "TableTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Private",
                value: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "TableTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "Private",
                value: true);

            migrationBuilder.UpdateData(
                table: "TableTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "Private",
                value: false);
        }
    }
}
