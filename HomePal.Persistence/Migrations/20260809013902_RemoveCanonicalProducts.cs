using System;
using Microsoft.Data.SqlTypes;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomePal.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCanonicalProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_CanonicalProducts_CanonicalProductId",
                table: "Offers");

            migrationBuilder.DropTable(
                name: "CanonicalProducts");

            migrationBuilder.DropIndex(
                name: "IX_Offers_CanonicalProductId",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "CanonicalProductId",
                table: "Offers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CanonicalProductId",
                table: "Offers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CanonicalProducts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Embedding = table.Column<SqlVector<float>>(type: "vector(1536)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CanonicalProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CanonicalProducts_ProductCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "ProductCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Offers_CanonicalProductId",
                table: "Offers",
                column: "CanonicalProductId");

            migrationBuilder.CreateIndex(
                name: "IX_CanonicalProducts_CategoryId",
                table: "CanonicalProducts",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_CanonicalProducts_CanonicalProductId",
                table: "Offers",
                column: "CanonicalProductId",
                principalTable: "CanonicalProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
