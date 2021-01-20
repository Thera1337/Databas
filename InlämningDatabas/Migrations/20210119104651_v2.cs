using Microsoft.EntityFrameworkCore.Migrations;

namespace InlämningDatabas.Migrations
{
    public partial class v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PositionForReading",
                table: "Temperature",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PositionForReading",
                table: "Temperature");
        }
    }
}
