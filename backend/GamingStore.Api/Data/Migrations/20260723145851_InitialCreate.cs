using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GamingStore.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sellers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sellers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SellerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    ReleaseDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Sellers",
                columns: new[] { "Id", "CreatedAt", "Description", "Email", "Name" },
                values: new object[] { new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), new DateTime(2026, 7, 23, 10, 0, 0, 0, DateTimeKind.Utc), "Independent seller focused on thoughtful PC games.", "studio@northbyte.local", "Northbyte Games" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName" },
                values: new object[] { new Guid("6d16f5fd-0e50-4e25-894c-5f2d5a767b7f"), new DateTime(2026, 7, 23, 10, 0, 0, 0, DateTimeKind.Utc), "alex.player@gamingstore.local", "Alex", "Player" });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "CreatedAt", "Description", "Price", "ReleaseDate", "SellerId", "Title" },
                values: new object[,]
                {
                    { new Guid("13a7712c-7f0b-4cb3-b9b1-09db733c4c5c"), new DateTime(2026, 7, 23, 10, 0, 0, 0, DateTimeKind.Utc), "A compact sci-fi strategy game about rebuilding a fleet after a failed jump.", 24.99m, new DateOnly(2025, 11, 14), new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), "Starfall Tactics" },
                    { new Guid("54acfc5b-694a-42c3-a21c-98188f1cf0a6"), new DateTime(2026, 7, 23, 10, 0, 0, 0, DateTimeKind.Utc), "Fast local arena battles with simple controls and sharp item timing.", 14.99m, new DateOnly(2026, 3, 8), new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), "Pixel Forge Arena" },
                    { new Guid("ab18590a-11fd-4269-9909-11aa2df97688"), new DateTime(2026, 7, 23, 10, 0, 0, 0, DateTimeKind.Utc), "A shopkeeping adventure where every item has a story and a margin.", 19.99m, new DateOnly(2024, 9, 27), new Guid("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"), "Dungeon Ledger" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Games_SellerId",
                table: "Games",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_Email",
                table: "Sellers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Sellers");
        }
    }
}
