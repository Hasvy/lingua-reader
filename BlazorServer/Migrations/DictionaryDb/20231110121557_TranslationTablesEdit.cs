using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.Migrations.DictionaryDb
{
    /// <inheritdoc />
    public partial class TranslationTablesEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropForeignKey(
                name: "FK_WordTranslations_TranslatorWordResponse_TranslatorWordResponseId",
                table: "WordTranslations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WordTranslations",
                table: "WordTranslations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TranslatorWordResponse",
                table: "TranslatorWordResponse");

            migrationBuilder.RenameTable(
                name: "WordTranslations",
                newName: "Translations");

            migrationBuilder.RenameTable(
                name: "TranslatorWordResponse",
                newName: "Words");

            migrationBuilder.RenameIndex(
                name: "IX_WordTranslations_TranslatorWordResponseId",
                table: "Translations",
                newName: "IX_Translations_TranslatorWordResponseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Translations",
                table: "Translations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Words",
                table: "Words",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_Words_TranslatorWordResponseId",
                table: "Translations",
                column: "TranslatorWordResponseId",
                principalTable: "Words",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationsEnglish_Translations_Id",
                table: "TranslationsEnglish",
                column: "Id",
                principalTable: "Translations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationsGerman_Translations_Id",
                table: "TranslationsGerman",
                column: "Id",
                principalTable: "Translations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TranslationsRussian_Translations_Id",
                table: "TranslationsRussian",
                column: "Id",
                principalTable: "Translations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Translations_Words_TranslatorWordResponseId",
                table: "Translations");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationsEnglish_Translations_Id",
                table: "TranslationsEnglish");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationsGerman_Translations_Id",
                table: "TranslationsGerman");

            migrationBuilder.DropForeignKey(
                name: "FK_TranslationsRussian_Translations_Id",
                table: "TranslationsRussian");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Words",
                table: "Words");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Translations",
                table: "Translations");

            migrationBuilder.RenameTable(
                name: "Words",
                newName: "TranslatorWordResponse");

            migrationBuilder.RenameTable(
                name: "Translations",
                newName: "WordTranslations");

            migrationBuilder.RenameIndex(
                name: "IX_Translations_TranslatorWordResponseId",
                table: "WordTranslations",
                newName: "IX_WordTranslations_TranslatorWordResponseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TranslatorWordResponse",
                table: "TranslatorWordResponse",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WordTranslations",
                table: "WordTranslations",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_WordTranslations_TranslatorWordResponse_TranslatorWordResponseId",
                table: "WordTranslations",
                column: "TranslatorWordResponseId",
                principalTable: "TranslatorWordResponse",
                principalColumn: "Id");
        }
    }
}
