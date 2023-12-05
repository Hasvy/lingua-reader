using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.Migrations.DictionaryDb
{
    /// <inheritdoc />
    public partial class AddTranslationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "prefixWord",
                table: "WordTranslations",
                newName: "PrefixWord");

            migrationBuilder.RenameColumn(
                name: "posTag",
                table: "WordTranslations",
                newName: "PosTag");

            migrationBuilder.RenameColumn(
                name: "displayTarget",
                table: "WordTranslations",
                newName: "DisplayTarget");

            migrationBuilder.RenameColumn(
                name: "confidence",
                table: "WordTranslations",
                newName: "Confidence");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "WordTranslations",
                newName: "WordId");

            migrationBuilder.AlterColumn<string>(
                name: "PosTag",
                table: "WordTranslations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TranslatorWordResponseId",
                table: "WordTranslations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TranslationsEnglish",
                columns: table => new
                {
                    WordId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationsEnglish", x => x.WordId);
                    table.ForeignKey(
                        name: "FK_TranslationsEnglish_WordTranslations_WordId",
                        column: x => x.WordId,
                        principalTable: "WordTranslations",
                        principalColumn: "WordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranslationsGerman",
                columns: table => new
                {
                    WordId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationsGerman", x => x.WordId);
                    table.ForeignKey(
                        name: "FK_TranslationsGerman_WordTranslations_WordId",
                        column: x => x.WordId,
                        principalTable: "WordTranslations",
                        principalColumn: "WordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranslationsRussian",
                columns: table => new
                {
                    WordId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationsRussian", x => x.WordId);
                    table.ForeignKey(
                        name: "FK_TranslationsRussian_WordTranslations_WordId",
                        column: x => x.WordId,
                        principalTable: "WordTranslations",
                        principalColumn: "WordId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TranslatorWordResponse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplaySource = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslatorWordResponse", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WordTranslations_TranslatorWordResponseId",
                table: "WordTranslations",
                column: "TranslatorWordResponseId");

            migrationBuilder.AddForeignKey(
                name: "FK_WordTranslations_TranslatorWordResponse_TranslatorWordResponseId",
                table: "WordTranslations",
                column: "TranslatorWordResponseId",
                principalTable: "TranslatorWordResponse",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WordTranslations_TranslatorWordResponse_TranslatorWordResponseId",
                table: "WordTranslations");

            migrationBuilder.DropTable(
                name: "TranslationsEnglish");

            migrationBuilder.DropTable(
                name: "TranslationsGerman");

            migrationBuilder.DropTable(
                name: "TranslationsRussian");

            migrationBuilder.DropTable(
                name: "TranslatorWordResponse");

            migrationBuilder.DropIndex(
                name: "IX_WordTranslations_TranslatorWordResponseId",
                table: "WordTranslations");

            migrationBuilder.DropColumn(
                name: "TranslatorWordResponseId",
                table: "WordTranslations");

            migrationBuilder.RenameColumn(
                name: "PrefixWord",
                table: "WordTranslations",
                newName: "prefixWord");

            migrationBuilder.RenameColumn(
                name: "PosTag",
                table: "WordTranslations",
                newName: "posTag");

            migrationBuilder.RenameColumn(
                name: "DisplayTarget",
                table: "WordTranslations",
                newName: "displayTarget");

            migrationBuilder.RenameColumn(
                name: "Confidence",
                table: "WordTranslations",
                newName: "confidence");

            migrationBuilder.RenameColumn(
                name: "WordId",
                table: "WordTranslations",
                newName: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "posTag",
                table: "WordTranslations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
