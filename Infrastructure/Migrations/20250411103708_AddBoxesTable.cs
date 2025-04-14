using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VR.Migrations
{
    /// <inheritdoc />
    public partial class AddBoxesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Boxes",
                columns: table => new
                {
                    Identifier = table.Column<string>(type: "text", nullable: false),
                    SupplierIdentifier = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boxes", x => x.Identifier);
                });

            migrationBuilder.CreateTable(
                name: "Contents",
                columns: table => new
                {
                    PoNumber = table.Column<string>(type: "text", nullable: false),
                    Isbn = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    BoxIdentifier = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contents", x => new { x.PoNumber, x.Isbn });
                    table.ForeignKey(
                        name: "FK_Contents_Boxes_BoxIdentifier",
                        column: x => x.BoxIdentifier,
                        principalTable: "Boxes",
                        principalColumn: "Identifier");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Contents_BoxIdentifier",
                table: "Contents",
                column: "BoxIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contents");

            migrationBuilder.DropTable(
                name: "Boxes");
        }
    }
}
