using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.Migrations
{
    /// <inheritdoc />
    public partial class AddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookSections_AbstractBooks_EpubBookId",
                table: "BookSections");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AbstractBooks");

            migrationBuilder.DropColumn(
                name: "SectionsCount",
                table: "AbstractBooks");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "AbstractBooks");

            migrationBuilder.CreateTable(
                name: "EpubBooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SectionsCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpubBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpubBooks_AbstractBooks_Id",
                        column: x => x.Id,
                        principalTable: "AbstractBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PdfBooks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PdfBooks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PdfBooks_AbstractBooks_Id",
                        column: x => x.Id,
                        principalTable: "AbstractBooks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BookSections_EpubBooks_EpubBookId",
                table: "BookSections",
                column: "EpubBookId",
                principalTable: "EpubBooks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookSections_EpubBooks_EpubBookId",
                table: "BookSections");

            migrationBuilder.DropTable(
                name: "EpubBooks");

            migrationBuilder.DropTable(
                name: "PdfBooks");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AbstractBooks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SectionsCount",
                table: "AbstractBooks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "AbstractBooks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookSections_AbstractBooks_EpubBookId",
                table: "BookSections",
                column: "EpubBookId",
                principalTable: "AbstractBooks",
                principalColumn: "Id");
        }
    }
}
