using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.Migrations.DictionaryDb
{
    /// <inheritdoc />
    public partial class EditTranslationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TranslationsEnglish_WordTranslations_WordId",
                table: "TranslationsEnglish");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationsGerman_WordTranslations_WordId",
                table: "TranslationsGerman");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationsRussian_WordTranslations_WordId",
                table: "TranslationsRussian");

            migrationBuilder.RenameColumn(
                name: "WordId",
                table: "WordTranslations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "WordId",
                table: "TranslationsRussian",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "WordId",
                table: "TranslationsGerman",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "WordId",
                table: "TranslationsEnglish",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationsEnglish_WordTranslations_Id",
                table: "TranslationsEnglish",
                column: "Id",
                principalTable: "WordTranslations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationsGerman_WordTranslations_Id",
                table: "TranslationsGerman",
                column: "Id",
                principalTable: "WordTranslations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationsRussian_WordTranslations_Id",
                table: "TranslationsRussian",
                column: "Id",
                principalTable: "WordTranslations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TranslationsEnglish_WordTranslations_Id",
                table: "TranslationsEnglish");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationsGerman_WordTranslations_Id",
                table: "TranslationsGerman");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationsRussian_WordTranslations_Id",
                table: "TranslationsRussian");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "WordTranslations",
                newName: "WordId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TranslationsRussian",
                newName: "WordId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TranslationsGerman",
                newName: "WordId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TranslationsEnglish",
                newName: "WordId");

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationsEnglish_WordTranslations_WordId",
                table: "TranslationsEnglish",
                column: "WordId",
                principalTable: "WordTranslations",
                principalColumn: "WordId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationsGerman_WordTranslations_WordId",
                table: "TranslationsGerman",
                column: "WordId",
                principalTable: "WordTranslations",
                principalColumn: "WordId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationsRussian_WordTranslations_WordId",
                table: "TranslationsRussian",
                column: "WordId",
                principalTable: "WordTranslations",
                principalColumn: "WordId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
