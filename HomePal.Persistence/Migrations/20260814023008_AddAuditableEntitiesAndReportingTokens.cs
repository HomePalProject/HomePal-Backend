using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePal.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditableEntitiesAndReportingTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Supermarkets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Supermarkets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ShoppingLists",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ShoppingLists",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ShoppingListItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ShoppingListItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ProductCategories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Preferences",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Preferences",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PreferenceCategories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PreferenceCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "PantryItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "PantryItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Pantries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Pantries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Offers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Offers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "MeasuringUnits",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MeasuringUnits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "MealPlans",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MealPlans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Households",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Households",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "HouseholdMonthlyBudgets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "HouseholdMonthlyBudgets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "HouseholdMembers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "HouseholdMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "HouseholdMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "HouseholdMembers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "HouseholdInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "HouseholdInvitations",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "HouseholdInvitations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "HouseholdExpenses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "HouseholdExpenses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AgentChatSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AgentChatSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TotalTokensUsed",
                table: "AgentChatSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AgentChatMessages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AgentChatMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "TokensUsed",
                table: "AgentChatMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AgentChatMessages",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Supermarkets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Supermarkets");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ShoppingLists");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ShoppingListItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ShoppingListItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductCategories");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Preferences");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Preferences");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PreferenceCategories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PreferenceCategories");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "PantryItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "PantryItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Pantries");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Pantries");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MeasuringUnits");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MeasuringUnits");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MealPlans");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MealPlans");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Households");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Households");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "HouseholdMonthlyBudgets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "HouseholdMonthlyBudgets");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "HouseholdMembers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "HouseholdMembers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "HouseholdMembers");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "HouseholdMembers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "HouseholdInvitations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "HouseholdInvitations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "HouseholdInvitations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "HouseholdExpenses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "HouseholdExpenses");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AgentChatSessions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AgentChatSessions");

            migrationBuilder.DropColumn(
                name: "TotalTokensUsed",
                table: "AgentChatSessions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AgentChatMessages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AgentChatMessages");

            migrationBuilder.DropColumn(
                name: "TokensUsed",
                table: "AgentChatMessages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AgentChatMessages");
        }
    }
}
