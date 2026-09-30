using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CivicConnect.Migrations
{
    /// <summary>
    /// FR-002: replaces the free-text Category column with a controlled Category lookup table.
    /// Existing requests are mapped to the matching category by name; anything that does
    /// not match is mapped to "Other" so no request data is lost.
    /// </summary>
    public partial class AddCategoryLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, true, "Maintenance" },
                    { 2, true, "IT Support" },
                    { 3, true, "Facilities" },
                    { 4, true, "Cleaning" },
                    { 5, true, "Security" },
                    { 6, true, "Other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            // Add as nullable first so existing rows can be mapped before the column becomes required.
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ServiceRequests",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
UPDATE sr SET sr.CategoryId = c.Id
FROM ServiceRequests sr
JOIN Categories c ON LOWER(LTRIM(RTRIM(sr.Category))) = LOWER(c.Name);

UPDATE ServiceRequests SET CategoryId = 6 WHERE CategoryId IS NULL;");

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "ServiceRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Category",
                table: "ServiceRequests");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_CategoryId",
                table: "ServiceRequests",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRequests_Categories_CategoryId",
                table: "ServiceRequests",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRequests_Categories_CategoryId",
                table: "ServiceRequests");

            migrationBuilder.DropIndex(
                name: "IX_ServiceRequests_CategoryId",
                table: "ServiceRequests");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(@"
UPDATE sr SET sr.Category = c.Name
FROM ServiceRequests sr
JOIN Categories c ON sr.CategoryId = c.Id;");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ServiceRequests");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
