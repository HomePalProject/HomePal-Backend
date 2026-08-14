using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePal.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorBudgetToBudgetDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HouseholdMonthlyBudgets_HouseholdId_Year_Month",
                table: "HouseholdMonthlyBudgets");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "HouseholdMonthlyBudgets");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "HouseholdMonthlyBudgets");

            migrationBuilder.AddColumn<DateTime>(
                name: "BudgetDate",
                table: "HouseholdMonthlyBudgets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdMonthlyBudgets_HouseholdId_BudgetDate",
                table: "HouseholdMonthlyBudgets",
                columns: new[] { "HouseholdId", "BudgetDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_HouseholdMonthlyBudgets_HouseholdId_BudgetDate",
                table: "HouseholdMonthlyBudgets");

            migrationBuilder.DropColumn(
                name: "BudgetDate",
                table: "HouseholdMonthlyBudgets");

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "HouseholdMonthlyBudgets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "HouseholdMonthlyBudgets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdMonthlyBudgets_HouseholdId_Year_Month",
                table: "HouseholdMonthlyBudgets",
                columns: new[] { "HouseholdId", "Year", "Month" },
                unique: true);
        }
    }
}
