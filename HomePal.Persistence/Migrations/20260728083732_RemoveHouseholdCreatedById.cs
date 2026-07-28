using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePal.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHouseholdCreatedById : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Households_AspNetUsers_CreatedById",
                table: "Households");

            migrationBuilder.DropIndex(
                name: "IX_Households_CreatedById",
                table: "Households");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Households");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Households",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Households_CreatedById",
                table: "Households",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Households_AspNetUsers_CreatedById",
                table: "Households",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
