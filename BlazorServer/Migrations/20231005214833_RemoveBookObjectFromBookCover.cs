using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBookObjectFromBookCover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookCovers_AbstractBooks_BookId",
                table: "BookCovers");

            migrationBuilder.DropIndex(
                name: "IX_BookCovers_BookId",
                table: "BookCovers");

            migrationBuilder.AddColumn<Guid>(
                name: "BookCoverId",
                table: "AbstractBooks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AbstractBooks_BookCoverId",
                table: "AbstractBooks",
                column: "BookCoverId");

            migrationBuilder.AddForeignKey(
                name: "FK_AbstractBooks_BookCovers_BookCoverId",
                table: "AbstractBooks",
                column: "BookCoverId",
                principalTable: "BookCovers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AbstractBooks_BookCovers_BookCoverId",
                table: "AbstractBooks");

            migrationBuilder.DropIndex(
                name: "IX_AbstractBooks_BookCoverId",
                table: "AbstractBooks");

            migrationBuilder.DropColumn(
                name: "BookCoverId",
                table: "AbstractBooks");

            migrationBuilder.CreateIndex(
                name: "IX_BookCovers_BookId",
                table: "BookCovers",
                column: "BookId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookCovers_AbstractBooks_BookId",
                table: "BookCovers",
                column: "BookId",
                principalTable: "AbstractBooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
