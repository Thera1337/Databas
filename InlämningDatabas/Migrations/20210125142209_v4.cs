using Microsoft.EntityFrameworkCore.Migrations;

namespace InlämningDatabas.Migrations
{
    public partial class v4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Temperature_Date_DateID",
                table: "Temperature");

            migrationBuilder.AlterColumn<int>(
                name: "DateID",
                table: "Temperature",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Temperature_Date_DateID",
                table: "Temperature",
                column: "DateID",
                principalTable: "Date",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Temperature_Date_DateID",
                table: "Temperature");

            migrationBuilder.AlterColumn<int>(
                name: "DateID",
                table: "Temperature",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Temperature_Date_DateID",
                table: "Temperature",
                column: "DateID",
                principalTable: "Date",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
