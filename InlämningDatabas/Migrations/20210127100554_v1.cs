using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace InlämningDatabas.Migrations
{
    public partial class v1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Date",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateOfTemperature = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Date", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Temperature",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateID = table.Column<int>(type: "int", nullable: false),
                    TemperatureReading = table.Column<float>(type: "real", nullable: false),
                    PositionForReading = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Humidity = table.Column<int>(type: "int", nullable: false),
                    TimeOfTemperatureReading = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateOfTemperatureReading = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temperature", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Temperature_Date_DateID",
                        column: x => x.DateID,
                        principalTable: "Date",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Temperature_DateID",
                table: "Temperature",
                column: "DateID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Temperature");

            migrationBuilder.DropTable(
                name: "Date");
        }
    }
}
