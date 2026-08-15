using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePal.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDeleteBehaviors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HouseholdExpenses_Households_HouseholdId",
                table: "HouseholdExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_HouseholdInvitations_AspNetUsers_InvitedById",
                table: "HouseholdInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryItems_MeasuringUnits_MeasuringUnitId",
                table: "PantryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryItems_ProductCategories_CategoryId",
                table: "PantryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Preferences_PreferenceCategories_CategoryId",
                table: "Preferences");

            migrationBuilder.AlterColumn<Guid>(
                name: "MeasuringUnitId",
                table: "PantryItems",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "PantryItems",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_HouseholdExpenses_Households_HouseholdId",
                table: "HouseholdExpenses",
                column: "HouseholdId",
                principalTable: "Households",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HouseholdInvitations_AspNetUsers_InvitedById",
                table: "HouseholdInvitations",
                column: "InvitedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PantryItems_MeasuringUnits_MeasuringUnitId",
                table: "PantryItems",
                column: "MeasuringUnitId",
                principalTable: "MeasuringUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_PantryItems_ProductCategories_CategoryId",
                table: "PantryItems",
                column: "CategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Preferences_PreferenceCategories_CategoryId",
                table: "Preferences",
                column: "CategoryId",
                principalTable: "PreferenceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HouseholdExpenses_Households_HouseholdId",
                table: "HouseholdExpenses");

            migrationBuilder.DropForeignKey(
                name: "FK_HouseholdInvitations_AspNetUsers_InvitedById",
                table: "HouseholdInvitations");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryItems_MeasuringUnits_MeasuringUnitId",
                table: "PantryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PantryItems_ProductCategories_CategoryId",
                table: "PantryItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Preferences_PreferenceCategories_CategoryId",
                table: "Preferences");

            migrationBuilder.AlterColumn<Guid>(
                name: "MeasuringUnitId",
                table: "PantryItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "PantryItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_HouseholdExpenses_Households_HouseholdId",
                table: "HouseholdExpenses",
                column: "HouseholdId",
                principalTable: "Households",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HouseholdInvitations_AspNetUsers_InvitedById",
                table: "HouseholdInvitations",
                column: "InvitedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PantryItems_MeasuringUnits_MeasuringUnitId",
                table: "PantryItems",
                column: "MeasuringUnitId",
                principalTable: "MeasuringUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PantryItems_ProductCategories_CategoryId",
                table: "PantryItems",
                column: "CategoryId",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Preferences_PreferenceCategories_CategoryId",
                table: "Preferences",
                column: "CategoryId",
                principalTable: "PreferenceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
