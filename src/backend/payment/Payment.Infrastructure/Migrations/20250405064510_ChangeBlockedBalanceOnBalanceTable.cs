using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBlockedBalanceOnBalanceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Orders_OrderId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payouts",
                table: "Payouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Balances",
                table: "Balances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "BlockedBalance",
                table: "Balances");

            migrationBuilder.RenameTable(
                name: "Transactions",
                newName: "transactions");

            migrationBuilder.RenameTable(
                name: "Payouts",
                newName: "payouts");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "payments");

            migrationBuilder.RenameTable(
                name: "Balances",
                newName: "balances");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "order");

            migrationBuilder.RenameTable(
                name: "OrderItems",
                newName: "order-items");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_OrderId",
                table: "transactions",
                newName: "IX_transactions_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderItems_OrderId",
                table: "order-items",
                newName: "IX_order-items_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_transactions",
                table: "transactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_payouts",
                table: "payouts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_payments",
                table: "payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_balances",
                table: "balances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_order",
                table: "order",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_order-items",
                table: "order-items",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_order-items_order_OrderId",
                table: "order-items",
                column: "OrderId",
                principalTable: "order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_order_OrderId",
                table: "transactions",
                column: "OrderId",
                principalTable: "order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order-items_order_OrderId",
                table: "order-items");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_order_OrderId",
                table: "transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_transactions",
                table: "transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payouts",
                table: "payouts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_payments",
                table: "payments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_balances",
                table: "balances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_order-items",
                table: "order-items");

            migrationBuilder.DropPrimaryKey(
                name: "PK_order",
                table: "order");

            migrationBuilder.RenameTable(
                name: "transactions",
                newName: "Transactions");

            migrationBuilder.RenameTable(
                name: "payouts",
                newName: "Payouts");

            migrationBuilder.RenameTable(
                name: "payments",
                newName: "Payments");

            migrationBuilder.RenameTable(
                name: "balances",
                newName: "Balances");

            migrationBuilder.RenameTable(
                name: "order-items",
                newName: "OrderItems");

            migrationBuilder.RenameTable(
                name: "order",
                newName: "Orders");

            migrationBuilder.RenameIndex(
                name: "IX_transactions_OrderId",
                table: "Transactions",
                newName: "IX_Transactions_OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_order-items_OrderId",
                table: "OrderItems",
                newName: "IX_OrderItems_OrderId");

            migrationBuilder.AddColumn<decimal>(
                name: "BlockedBalance",
                table: "Balances",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Transactions",
                table: "Transactions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payouts",
                table: "Payouts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Balances",
                table: "Balances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderItems",
                table: "OrderItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_Orders_OrderId",
                table: "OrderItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Orders_OrderId",
                table: "Transactions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
