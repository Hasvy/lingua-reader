using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.Migrations
{
    /// <inheritdoc />
    public partial class Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BookCovers",
                keyColumn: "Id",
                keyValue: new Guid("1ef258bf-f1dc-4a97-be19-37371614b223"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("a4e70e8a-f88e-44eb-8534-1819f7b94e15"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Text" },
                values: new object[] { new Guid("a4e70e8a-f88e-44eb-8534-1819f7b94e15"), "Text" });

            migrationBuilder.InsertData(
                table: "BookCovers",
                columns: new[] { "Id", "Author", "BookId", "Description", "Format", "Title" },
                values: new object[] { new Guid("1ef258bf-f1dc-4a97-be19-37371614b223"), "Author", new Guid("a4e70e8a-f88e-44eb-8534-1819f7b94e15"), "Description", "epub", "Title2" });
        }
    }
}
