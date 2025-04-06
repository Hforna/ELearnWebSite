using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Payment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateBlockedBalance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order-items_order_OrderId",
                table: "order-items");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_order_OrderId",
                table: "transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_order",
                table: "order");

            migrationBuilder.RenameTable(
                name: "order",
                newName: "orders");

            migrationBuilder.AddPrimaryKey(
                name: "PK_orders",
                table: "orders",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "blocked-balances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BalanceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_blocked-balances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_blocked-balances_balances_BalanceId",
                        column: x => x.BalanceId,
                        principalTable: "balances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_blocked-balances_BalanceId",
                table: "blocked-balances",
                column: "BalanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_order-items_orders_OrderId",
                table: "order-items",
                column: "OrderId",
                principalTable: "orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_transactions_orders_OrderId",
                table: "transactions",
                column: "OrderId",
                principalTable: "orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order-items_orders_OrderId",
                table: "order-items");

            migrationBuilder.DropForeignKey(
                name: "FK_transactions_orders_OrderId",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "blocked-balances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_orders",
                table: "orders");

            migrationBuilder.RenameTable(
                name: "orders",
                newName: "order");

            migrationBuilder.AddPrimaryKey(
                name: "PK_order",
                table: "order",
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
    }
}
