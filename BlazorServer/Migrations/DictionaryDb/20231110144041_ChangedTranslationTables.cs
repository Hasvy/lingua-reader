using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorServer.Migrations.DictionaryDb
{
    /// <inheritdoc />
    public partial class ChangedTranslationTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Translations_Words_TranslatorWordResponseId",
                table: "Translations");

            migrationBuilder.DropIndex(
                name: "IX_Translations_TranslatorWordResponseId",
                table: "Translations");

            migrationBuilder.DropColumn(
                name: "TranslatorWordResponseId",
                table: "Translations");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "Words",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "WordId",
                table: "Translations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Translations_WordId",
                table: "Translations",
                column: "WordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_Words_WordId",
                table: "Translations",
                column: "WordId",
                principalTable: "Words",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Translations_Words_WordId",
                table: "Translations");

            migrationBuilder.DropIndex(
                name: "IX_Translations_WordId",
                table: "Translations");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "WordId",
                table: "Translations");

            migrationBuilder.AddColumn<int>(
                name: "TranslatorWordResponseId",
                table: "Translations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Translations_TranslatorWordResponseId",
                table: "Translations",
                column: "TranslatorWordResponseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Translations_Words_TranslatorWordResponseId",
                table: "Translations",
                column: "TranslatorWordResponseId",
                principalTable: "Words",
                principalColumn: "Id");
        }
    }
}
