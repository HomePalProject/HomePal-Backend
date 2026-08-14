using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePal.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTokensAndSyncAuditableEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalTokensUsed",
                table: "AgentChatSessions");

            migrationBuilder.DropColumn(
                name: "TokensUsed",
                table: "AgentChatMessages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalTokensUsed",
                table: "AgentChatSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TokensUsed",
                table: "AgentChatMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
