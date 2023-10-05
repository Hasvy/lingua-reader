using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.Migrations
{
    /// <inheritdoc />
    public partial class AddEpubBookId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookSections_EpubBooks_EpubBookId",
                table: "BookSections");

            migrationBuilder.AlterColumn<Guid>(
                name: "EpubBookId",
                table: "BookSections",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BookSections_EpubBooks_EpubBookId",
                table: "BookSections",
                column: "EpubBookId",
                principalTable: "EpubBooks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookSections_EpubBooks_EpubBookId",
                table: "BookSections");

            migrationBuilder.AlterColumn<Guid>(
                name: "EpubBookId",
                table: "BookSections",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_BookSections_EpubBooks_EpubBookId",
                table: "BookSections",
                column: "EpubBookId",
                principalTable: "EpubBooks",
                principalColumn: "Id");
        }
    }
}
