using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePal.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMealPlanIdToShoppingListItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MealPlanId",
                table: "ShoppingListItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListItems_MealPlanId",
                table: "ShoppingListItems",
                column: "MealPlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingListItems_MealPlans_MealPlanId",
                table: "ShoppingListItems",
                column: "MealPlanId",
                principalTable: "MealPlans",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingListItems_MealPlans_MealPlanId",
                table: "ShoppingListItems");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingListItems_MealPlanId",
                table: "ShoppingListItems");

            migrationBuilder.DropColumn(
                name: "MealPlanId",
                table: "ShoppingListItems");
        }
    }
}
