using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserMainLanguage",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserMainLanguage",
                table: "AspNetUsers");
        }
    }
}
