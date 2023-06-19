using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApplicationCore.Migrations
{
    public partial class addfktable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tables",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "TableStatuses",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 1, "Vacant" },
                    { 2, "Occupied" },
                    { 3, "Unavailable" }
                });

            migrationBuilder.InsertData(
                table: "TableTypes",
                columns: new[] { "Id", "Private", "Seat" },
                values: new object[,]
                {
                    { 1, true, 2 },
                    { 2, true, 2 },
                    { 3, false, 4 },
                    { 4, false, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tables_StatusId",
                table: "Tables",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Tables_TypeId",
                table: "Tables",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_TableStatuses_StatusId",
                table: "Tables",
                column: "StatusId",
                principalTable: "TableStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_TableTypes_TypeId",
                table: "Tables",
                column: "TypeId",
                principalTable: "TableTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_TableStatuses_StatusId",
                table: "Tables");

            migrationBuilder.DropForeignKey(
                name: "FK_Tables_TableTypes_TypeId",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_StatusId",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_TypeId",
                table: "Tables");

            migrationBuilder.DeleteData(
                table: "TableStatuses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TableStatuses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TableStatuses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TableTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TableTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TableTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TableTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tables");
        }
    }
}
