using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApplicationContext.Migrations
{
    public partial class adjustdb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_ReservationStatus_StatusId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Tables_TableStatuses_StatusId",
                table: "Tables");

            migrationBuilder.DropTable(
                name: "ReservationStatus");

            migrationBuilder.DropTable(
                name: "TableStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Tables_StatusId",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_StatusId",
                table: "Reservations");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Reservations",
                newName: "Status");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Tables",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Tables");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Reservations",
                newName: "StatusId");

            migrationBuilder.CreateTable(
                name: "ReservationStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TableStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableStatuses", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TableStatuses",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 1, "Vacant" },
                    { 2, "Occupied" },
                    { 3, "Unavailable" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tables_StatusId",
                table: "Tables",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_StatusId",
                table: "Reservations",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_ReservationStatus_StatusId",
                table: "Reservations",
                column: "StatusId",
                principalTable: "ReservationStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_TableStatuses_StatusId",
                table: "Tables",
                column: "StatusId",
                principalTable: "TableStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
