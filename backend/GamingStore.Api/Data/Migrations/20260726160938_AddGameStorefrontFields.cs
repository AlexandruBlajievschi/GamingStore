using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamingStore.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGameStorefrontFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverImageUrl",
                table: "Games",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Games",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("13a7712c-7f0b-4cb3-b9b1-09db733c4c5c"),
                columns: new[] { "CoverImageUrl", "Description", "Slug", "Title" },
                values: new object[] { "/images/games/auralith-drift.webp", "Chart a silent alien ocean beneath luminous mineral rings and uncover why they are falling.", "auralith-drift", "Auralith Drift" });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("54acfc5b-694a-42c3-a21c-98188f1cf0a6"),
                columns: new[] { "CoverImageUrl", "Slug" },
                values: new object[] { null, "pixel-forge-arena" });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("ab18590a-11fd-4269-9909-11aa2df97688"),
                columns: new[] { "CoverImageUrl", "Slug" },
                values: new object[] { null, "dungeon-ledger" });

            migrationBuilder.Sql(
                """
                UPDATE "Games"
                SET "Slug" =
                    left(
                        COALESCE(
                            NULLIF(
                                trim(BOTH '-' FROM lower(regexp_replace("Title", '[^a-zA-Z0-9]+', '-', 'g'))),
                                ''),
                            'game'),
                        190)
                    || '-'
                    || left(replace("Id"::text, '-', ''), 8)
                WHERE "Slug" IS NULL;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Slug",
                table: "Games",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_Slug",
                table: "Games",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Games_Slug",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "CoverImageUrl",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Games");

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("13a7712c-7f0b-4cb3-b9b1-09db733c4c5c"),
                columns: new[] { "Description", "Title" },
                values: new object[] { "A compact sci-fi strategy game about rebuilding a fleet after a failed jump.", "Starfall Tactics" });
        }
    }
}
