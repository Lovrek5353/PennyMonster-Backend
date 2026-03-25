using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PennyMonster.Migrations
{
    /// <inheritdoc />
    public partial class FixDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavingPocket_Users_UserId",
                table: "SavingPocket");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscription_Users_UserId",
                table: "Subscription");

            migrationBuilder.DropForeignKey(
                name: "FK_Tab_Users_UserId",
                table: "Tab");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_SavingPocket_SavingPocketId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Subscription_SubscriptionId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Tab_TabId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tab",
                table: "Tab");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavingPocket",
                table: "SavingPocket");

            migrationBuilder.RenameTable(
                name: "Tab",
                newName: "Tabs");

            migrationBuilder.RenameTable(
                name: "Subscription",
                newName: "Subscriptions");

            migrationBuilder.RenameTable(
                name: "SavingPocket",
                newName: "SavingPockets");

            migrationBuilder.RenameColumn(
                name: "lender",
                table: "Tabs",
                newName: "Lender");

            migrationBuilder.RenameIndex(
                name: "IX_Tab_UserId",
                table: "Tabs",
                newName: "IX_Tabs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscription_UserId",
                table: "Subscriptions",
                newName: "IX_Subscriptions_UserId");

            migrationBuilder.RenameColumn(
                name: "color",
                table: "SavingPockets",
                newName: "Color");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "SavingPockets",
                newName: "StartDate");

            migrationBuilder.RenameIndex(
                name: "IX_SavingPocket_UserId",
                table: "SavingPockets",
                newName: "IX_SavingPockets_UserId");

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyPayment",
                table: "Tabs",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tabs",
                table: "Tabs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavingPockets",
                table: "SavingPockets",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavingPockets_Users_UserId",
                table: "SavingPockets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_UserId",
                table: "Subscriptions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tabs_Users_UserId",
                table: "Tabs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_SavingPockets_SavingPocketId",
                table: "Transactions",
                column: "SavingPocketId",
                principalTable: "SavingPockets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Subscriptions_SubscriptionId",
                table: "Transactions",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Tabs_TabId",
                table: "Transactions",
                column: "TabId",
                principalTable: "Tabs",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SavingPockets_Users_UserId",
                table: "SavingPockets");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_UserId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tabs_Users_UserId",
                table: "Tabs");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_SavingPockets_SavingPocketId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Subscriptions_SubscriptionId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Tabs_TabId",
                table: "Transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tabs",
                table: "Tabs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subscriptions",
                table: "Subscriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SavingPockets",
                table: "SavingPockets");

            migrationBuilder.DropColumn(
                name: "MonthlyPayment",
                table: "Tabs");

            migrationBuilder.RenameTable(
                name: "Tabs",
                newName: "Tab");

            migrationBuilder.RenameTable(
                name: "Subscriptions",
                newName: "Subscription");

            migrationBuilder.RenameTable(
                name: "SavingPockets",
                newName: "SavingPocket");

            migrationBuilder.RenameColumn(
                name: "Lender",
                table: "Tab",
                newName: "lender");

            migrationBuilder.RenameIndex(
                name: "IX_Tabs_UserId",
                table: "Tab",
                newName: "IX_Tab_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscription",
                newName: "IX_Subscription_UserId");

            migrationBuilder.RenameColumn(
                name: "Color",
                table: "SavingPocket",
                newName: "color");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "SavingPocket",
                newName: "CreatedDate");

            migrationBuilder.RenameIndex(
                name: "IX_SavingPockets_UserId",
                table: "SavingPocket",
                newName: "IX_SavingPocket_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tab",
                table: "Tab",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subscription",
                table: "Subscription",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SavingPocket",
                table: "SavingPocket",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SavingPocket_Users_UserId",
                table: "SavingPocket",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscription_Users_UserId",
                table: "Subscription",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tab_Users_UserId",
                table: "Tab",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_SavingPocket_SavingPocketId",
                table: "Transactions",
                column: "SavingPocketId",
                principalTable: "SavingPocket",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Subscription_SubscriptionId",
                table: "Transactions",
                column: "SubscriptionId",
                principalTable: "Subscription",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Tab_TabId",
                table: "Transactions",
                column: "TabId",
                principalTable: "Tab",
                principalColumn: "Id");
        }
    }
}
