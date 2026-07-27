using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GamingStore.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreSeededGames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "CoverImageUrl", "CreatedAt", "Description", "Price", "ReleaseDate", "SellerId", "Slug", "Title" },
                values: new object[,]
                {
                    { new Guid("1f1e36aa-4a2f-4f5b-9c42-57235fc18d03"), "/images/games/iron-vale-rally.webp", new DateTime(2026, 7, 27, 10, 0, 0, 0, DateTimeKind.Utc), "Drift armored cars through mountain switchbacks, industrial shortcuts, and weather-battered rally stages.", 39.99m, new DateOnly(2026, 2, 12), new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), "iron-vale-rally", "Iron Vale Rally" },
                    { new Guid("4cdb6a49-c2de-4b1c-8fbd-f6951620a609"), "/images/games/glacier-guild.webp", new DateTime(2026, 7, 27, 10, 0, 0, 0, DateTimeKind.Utc), "Gather a crew, craft in the ice, and build impossible machines before the glacier city shifts again.", 34.99m, new DateOnly(2026, 6, 11), new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), "glacier-guild", "Glacier Guild" },
                    { new Guid("63db3015-067e-4438-ae55-f7bb7603a207"), "/images/games/glass-planet-survey.webp", new DateTime(2026, 7, 27, 10, 0, 0, 0, DateTimeKind.Utc), "Map crystal canyons, scan alien minerals, and keep your expedition calm when the planet starts answering back.", 27.99m, new DateOnly(2025, 10, 22), new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), "glass-planet-survey", "Glass Planet Survey" },
                    { new Guid("7e62c4f2-0f4d-4f70-a407-0efc39b18e01"), "/images/games/neon-orchard.webp", new DateTime(2026, 7, 27, 10, 0, 0, 0, DateTimeKind.Utc), "Explore a cyberpunk orchard where glowing fruit, city drones, and lost signals point toward a buried secret.", 29.99m, new DateOnly(2026, 5, 19), new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), "neon-orchard", "Neon Orchard" },
                    { new Guid("9b7e98d8-1d43-4d30-890d-3bb57cbf2d02"), "/images/games/starfall-kitchen.webp", new DateTime(2026, 7, 27, 10, 0, 0, 0, DateTimeKind.Utc), "Cook under zero gravity, chase floating ingredients, and serve strange comfort food across a tiny starship route.", 17.99m, new DateOnly(2025, 8, 7), new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), "starfall-kitchen", "Starfall Kitchen" },
                    { new Guid("a55be02c-f531-472f-a763-4598b470cb10"), "/images/games/midnight-parcel.webp", new DateTime(2026, 7, 27, 10, 0, 0, 0, DateTimeKind.Utc), "Deliver fragile packages across rain-soaked rooftops while rival couriers chase the same impossible route.", 9.99m, new DateOnly(2025, 2, 25), new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), "midnight-parcel", "Midnight Parcel" },
                    { new Guid("b2074502-1057-463a-a90a-22a7a6db6a05"), "/images/games/velvet-circuit.webp", new DateTime(2026, 7, 27, 10, 0, 0, 0, DateTimeKind.Utc), "Race anti-gravity bikes through luxury neon circuits where boost timing matters as much as clean lines.", 21.99m, new DateOnly(2025, 4, 18), new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), "velvet-circuit", "Velvet Circuit" },
                    { new Guid("ca79ef4a-a356-4e9f-9d4e-759a3ee61c04"), "/images/games/rune-harbor.webp", new DateTime(2026, 7, 27, 10, 0, 0, 0, DateTimeKind.Utc), "Defend a stormlit port with sea magic, rune lighthouses, and choices that decide which ships make it home.", 24.99m, new DateOnly(2024, 12, 3), new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), "rune-harbor", "Rune Harbor" },
                    { new Guid("ed51ad9d-e182-4b74-a24c-b7a7ff422d06"), "/images/games/tiny-titan-tactics.webp", new DateTime(2026, 7, 27, 10, 0, 0, 0, DateTimeKind.Utc), "Command miniature mechs across tabletop battlefields where dice, blocks, and clever positioning rule the day.", 12.99m, new DateOnly(2026, 1, 30), new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), "tiny-titan-tactics", "Tiny Titan Tactics" },
                    { new Guid("f8f50b0e-7d23-48a0-8527-dfef40ea7208"), "/images/games/ember-library.webp", new DateTime(2026, 7, 27, 10, 0, 0, 0, DateTimeKind.Utc), "Solve warm, dangerous puzzles in an ancient library where every burning book remembers a different truth.", 16.99m, new DateOnly(2024, 11, 9), new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), "ember-library", "Ember Library" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("1f1e36aa-4a2f-4f5b-9c42-57235fc18d03"));

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("4cdb6a49-c2de-4b1c-8fbd-f6951620a609"));

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("63db3015-067e-4438-ae55-f7bb7603a207"));

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("7e62c4f2-0f4d-4f70-a407-0efc39b18e01"));

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("9b7e98d8-1d43-4d30-890d-3bb57cbf2d02"));

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("a55be02c-f531-472f-a763-4598b470cb10"));

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("b2074502-1057-463a-a90a-22a7a6db6a05"));

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("ca79ef4a-a356-4e9f-9d4e-759a3ee61c04"));

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("ed51ad9d-e182-4b74-a24c-b7a7ff422d06"));

            migrationBuilder.DeleteData(
                table: "Games",
                keyColumn: "Id",
                keyValue: new Guid("f8f50b0e-7d23-48a0-8527-dfef40ea7208"));
        }
    }
}
