using Microsoft.EntityFrameworkCore.Migrations;

namespace Algoriza2.EF.Migrations
{
    public partial class add_value_coupon : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "DiscountCodeCoupons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "DiscountCodeCoupons");
        }
    }
}
