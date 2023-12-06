using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.Migrations.DictionaryDb
{
    /// <inheritdoc />
    public partial class AddedLangColumnToTranslations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TranslationsEnglish");

            migrationBuilder.DropTable(
                name: "TranslationsGerman");

            migrationBuilder.DropTable(
                name: "TranslationsRussian");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Translations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "Translations");

            migrationBuilder.CreateTable(
                name: "TranslationsEnglish",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationsEnglish", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranslationsEnglish_Translations_Id",
                        column: x => x.Id,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranslationsGerman",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationsGerman", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranslationsGerman_Translations_Id",
                        column: x => x.Id,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranslationsRussian",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationsRussian", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranslationsRussian_Translations_Id",
                        column: x => x.Id,
                        principalTable: "Translations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
