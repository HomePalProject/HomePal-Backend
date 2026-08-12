using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePal.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHouseholdBudgetAndExpenses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HouseholdMonthlyBudgets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HouseholdId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdMonthlyBudgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HouseholdMonthlyBudgets_Households_HouseholdId",
                        column: x => x.HouseholdId,
                        principalTable: "Households",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HouseholdExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HouseholdId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BudgetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HouseholdExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HouseholdExpenses_HouseholdMonthlyBudgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "HouseholdMonthlyBudgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_HouseholdExpenses_Households_HouseholdId",
                        column: x => x.HouseholdId,
                        principalTable: "Households",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdExpenses_BudgetId",
                table: "HouseholdExpenses",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdExpenses_HouseholdId",
                table: "HouseholdExpenses",
                column: "HouseholdId");

            migrationBuilder.CreateIndex(
                name: "IX_HouseholdMonthlyBudgets_HouseholdId_Year_Month",
                table: "HouseholdMonthlyBudgets",
                columns: new[] { "HouseholdId", "Year", "Month" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HouseholdExpenses");

            migrationBuilder.DropTable(
                name: "HouseholdMonthlyBudgets");
        }
    }
}
