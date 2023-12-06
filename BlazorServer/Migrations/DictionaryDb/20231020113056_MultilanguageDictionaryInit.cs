using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.Migrations.DictionaryDb
{
    /// <inheritdoc />
    public partial class MultilanguageDictionaryInit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WordTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    displayTarget = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    posTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    confidence = table.Column<float>(type: "real", nullable: false),
                    prefixWord = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordTranslations", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordTranslations");
        }
    }
}
