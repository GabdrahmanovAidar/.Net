using Microsoft.EntityFrameworkCore.Migrations;

namespace MayakElectronics.Migrations
{
    public partial class Status_Corier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "OrdersCourier",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "OrdersCourier");
        }
    }
}
