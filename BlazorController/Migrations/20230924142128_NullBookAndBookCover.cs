using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.Migrations
{
    /// <inheritdoc />
    public partial class NullBookAndBookCover : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BookCovers",
                keyColumn: "Id",
                keyValue: new Guid("0cf6923b-9a81-4e0e-89f1-744165b9f19c"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("d87ea8ac-7a38-4b4d-a229-c8930203249e"));

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Text" },
                values: new object[] { new Guid("a4e70e8a-f88e-44eb-8534-1819f7b94e15"), "Text" });

            migrationBuilder.InsertData(
                table: "BookCovers",
                columns: new[] { "Id", "Author", "BookId", "Description", "Format", "Title" },
                values: new object[] { new Guid("1ef258bf-f1dc-4a97-be19-37371614b223"), "Author", new Guid("a4e70e8a-f88e-44eb-8534-1819f7b94e15"), "Description", "epub", "Title2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BookCovers",
                keyColumn: "Id",
                keyValue: new Guid("1ef258bf-f1dc-4a97-be19-37371614b223"));

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: new Guid("a4e70e8a-f88e-44eb-8534-1819f7b94e15"));

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Text" },
                values: new object[] { new Guid("d87ea8ac-7a38-4b4d-a229-c8930203249e"), "Text" });

            migrationBuilder.InsertData(
                table: "BookCovers",
                columns: new[] { "Id", "Author", "BookId", "Description", "Format", "Title" },
                values: new object[] { new Guid("0cf6923b-9a81-4e0e-89f1-744165b9f19c"), "Author", new Guid("d87ea8ac-7a38-4b4d-a229-c8930203249e"), "Description", "epub", "Title2" });
        }
    }
}
