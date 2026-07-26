using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GamingStore.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFictionalGameCovers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("54acfc5b-694a-42c3-a21c-98188f1cf0a6"),
                columns: new[] { "CoverImageUrl", "Description", "Slug", "Title" },
                values: new object[] { "/images/games/cindervolt-coliseum.webp", "Battle rival magnetic drones around a living forge where every power-up reshapes the arena.", "cindervolt-coliseum", "Cindervolt Coliseum" });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("ab18590a-11fd-4269-9909-11aa2df97688"),
                columns: new[] { "CoverImageUrl", "Description", "Slug", "Title" },
                values: new object[] { "/images/games/mosswick-and-mooncoin.webp", "Run a woodland curiosity shop where every enchanted object arrives with a story and a price.", "mosswick-and-mooncoin", "Mosswick & Mooncoin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("54acfc5b-694a-42c3-a21c-98188f1cf0a6"),
                columns: new[] { "CoverImageUrl", "Description", "Slug", "Title" },
                values: new object[] { null, "Fast local arena battles with simple controls and sharp item timing.", "pixel-forge-arena", "Pixel Forge Arena" });

            migrationBuilder.UpdateData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("ab18590a-11fd-4269-9909-11aa2df97688"),
                columns: new[] { "CoverImageUrl", "Description", "Slug", "Title" },
                values: new object[] { null, "A shopkeeping adventure where every item has a story and a margin.", "dungeon-ledger", "Dungeon Ledger" });
        }
    }
}
