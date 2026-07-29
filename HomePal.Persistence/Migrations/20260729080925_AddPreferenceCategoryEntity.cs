using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePal.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPreferenceCategoryEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                table: "Preferences",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PreferenceCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreferenceCategories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Preferences_CategoryId",
                table: "Preferences",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PreferenceCategories_Name",
                table: "PreferenceCategories",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Preferences_PreferenceCategories_CategoryId",
                table: "Preferences",
                column: "CategoryId",
                principalTable: "PreferenceCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Preferences_PreferenceCategories_CategoryId",
                table: "Preferences");

            migrationBuilder.DropTable(
                name: "PreferenceCategories");

            migrationBuilder.DropIndex(
                name: "IX_Preferences_CategoryId",
                table: "Preferences");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Preferences");
        }
    }
}
