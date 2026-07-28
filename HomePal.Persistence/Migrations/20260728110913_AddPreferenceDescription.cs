using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePal.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPreferenceDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Preferences",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Preferences");
        }
    }
}
