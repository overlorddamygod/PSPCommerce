using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSPCommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class orderidint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Order__OrderID",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem__OrderID",
                table: "OrderItem");

            migrationBuilder.DropColumn(
                name: "_OrderID",
                table: "OrderItem");

            migrationBuilder.AlterColumn<int>(
                name: "OrderID",
                table: "OrderItem",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_OrderID",
                table: "OrderItem",
                column: "OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Order_OrderID",
                table: "OrderItem",
                column: "OrderID",
                principalTable: "Order",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItem_Order_OrderID",
                table: "OrderItem");

            migrationBuilder.DropIndex(
                name: "IX_OrderItem_OrderID",
                table: "OrderItem");

            migrationBuilder.AlterColumn<string>(
                name: "OrderID",
                table: "OrderItem",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "_OrderID",
                table: "OrderItem",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem__OrderID",
                table: "OrderItem",
                column: "_OrderID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItem_Order__OrderID",
                table: "OrderItem",
                column: "_OrderID",
                principalTable: "Order",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
