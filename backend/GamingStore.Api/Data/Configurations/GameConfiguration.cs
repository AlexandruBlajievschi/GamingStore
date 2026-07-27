namespace GamingStore.Api.Data.Configurations;

public sealed class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("Games");

        builder.HasKey(game => game.Id);

        builder.Property(game => game.Slug)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(game => game.Slug)
            .IsUnique();

        builder.Property(game => game.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(game => game.Description)
            .HasMaxLength(2000);

        builder.Property(game => game.Price)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(game => game.CoverImageUrl)
            .HasMaxLength(2048);

        builder.Property(game => game.CreatedAt)
            .IsRequired();

        builder.HasOne(game => game.Seller)
            .WithMany(seller => seller.Games)
            .HasForeignKey(game => game.SellerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new
            {
                Id = Guid.Parse("13a7712c-7f0b-4cb3-b9b1-09db733c4c5c"),
                SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Slug = "auralith-drift",
                Title = "Auralith Drift",
                Description = "Chart a silent alien ocean beneath luminous mineral rings and uncover why they are falling.",
                Price = 24.99m,
                ReleaseDate = new DateOnly(2025, 11, 14),
                CoverImageUrl = "/images/games/auralith-drift.webp",
                CreatedAt = new DateTime(2026, 7, 23, 10, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("54acfc5b-694a-42c3-a21c-98188f1cf0a6"),
                SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Slug = "cindervolt-coliseum",
                Title = "Cindervolt Coliseum",
                Description = "Battle rival magnetic drones around a living forge where every power-up reshapes the arena.",
                Price = 14.99m,
                ReleaseDate = new DateOnly(2026, 3, 8),
                CoverImageUrl = "/images/games/cindervolt-coliseum.webp",
                CreatedAt = new DateTime(2026, 7, 23, 10, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("ab18590a-11fd-4269-9909-11aa2df97688"),
                SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Slug = "mosswick-and-mooncoin",
                Title = "Mosswick & Mooncoin",
                Description = "Run a woodland curiosity shop where every enchanted object arrives with a story and a price.",
                Price = 19.99m,
                ReleaseDate = new DateOnly(2024, 9, 27),
                CoverImageUrl = "/images/games/mosswick-and-mooncoin.webp",
                CreatedAt = new DateTime(2026, 7, 23, 10, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("7e62c4f2-0f4d-4f70-a407-0efc39b18e01"),
                SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Slug = "neon-orchard",
                Title = "Neon Orchard",
                Description = "Explore a cyberpunk orchard where glowing fruit, city drones, and lost signals point toward a buried secret.",
                Price = 29.99m,
                ReleaseDate = new DateOnly(2026, 5, 19),
                CoverImageUrl = "/images/games/neon-orchard.webp",
                CreatedAt = new DateTime(2026, 7, 27, 10, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("9b7e98d8-1d43-4d30-890d-3bb57cbf2d02"),
                SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Slug = "starfall-kitchen",
                Title = "Starfall Kitchen",
                Description = "Cook under zero gravity, chase floating ingredients, and serve strange comfort food across a tiny starship route.",
                Price = 17.99m,
                ReleaseDate = new DateOnly(2025, 8, 7),
                CoverImageUrl = "/images/games/starfall-kitchen.webp",
                CreatedAt = new DateTime(2026, 7, 27, 10, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("1f1e36aa-4a2f-4f5b-9c42-57235fc18d03"),
                SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Slug = "iron-vale-rally",
                Title = "Iron Vale Rally",
                Description = "Drift armored cars through mountain switchbacks, industrial shortcuts, and weather-battered rally stages.",
                Price = 39.99m,
                ReleaseDate = new DateOnly(2026, 2, 12),
                CoverImageUrl = "/images/games/iron-vale-rally.webp",
                CreatedAt = new DateTime(2026, 7, 27, 10, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("ca79ef4a-a356-4e9f-9d4e-759a3ee61c04"),
                SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Slug = "rune-harbor",
                Title = "Rune Harbor",
                Description = "Defend a stormlit port with sea magic, rune lighthouses, and choices that decide which ships make it home.",
                Price = 24.99m,
                ReleaseDate = new DateOnly(2024, 12, 3),
                CoverImageUrl = "/images/games/rune-harbor.webp",
                CreatedAt = new DateTime(2026, 7, 27, 10, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("b2074502-1057-463a-a90a-22a7a6db6a05"),
                SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Slug = "velvet-circuit",
                Title = "Velvet Circuit",
                Description = "Race anti-gravity bikes through luxury neon circuits where boost timing matters as much as clean lines.",
                Price = 21.99m,
                ReleaseDate = new DateOnly(2025, 4, 18),
                CoverImageUrl = "/images/games/velvet-circuit.webp",
                CreatedAt = new DateTime(2026, 7, 27, 10, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("ed51ad9d-e182-4b74-a24c-b7a7ff422d06"),
                SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Slug = "tiny-titan-tactics",
                Title = "Tiny Titan Tactics",
                Description = "Command miniature mechs across tabletop battlefields where dice, blocks, and clever positioning rule the day.",
                Price = 12.99m,
                ReleaseDate = new DateOnly(2026, 1, 30),
                CoverImageUrl = "/images/games/tiny-titan-tactics.webp",
                CreatedAt = new DateTime(2026, 7, 27, 10, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("63db3015-067e-4438-ae55-f7bb7603a207"),
                SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Slug = "glass-planet-survey",
                Title = "Glass Planet Survey",
                Description = "Map crystal canyons, scan alien minerals, and keep your expedition calm when the planet starts answering back.",
                Price = 27.99m,
                ReleaseDate = new DateOnly(2025, 10, 22),
                CoverImageUrl = "/images/games/glass-planet-survey.webp",
                CreatedAt = new DateTime(2026, 7, 27, 10, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("f8f50b0e-7d23-48a0-8527-dfef40ea7208"),
                SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Slug = "ember-library",
                Title = "Ember Library",
                Description = "Solve warm, dangerous puzzles in an ancient library where every burning book remembers a different truth.",
                Price = 16.99m,
                ReleaseDate = new DateOnly(2024, 11, 9),
                CoverImageUrl = "/images/games/ember-library.webp",
                CreatedAt = new DateTime(2026, 7, 27, 10, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("4cdb6a49-c2de-4b1c-8fbd-f6951620a609"),
                SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Slug = "glacier-guild",
                Title = "Glacier Guild",
                Description = "Gather a crew, craft in the ice, and build impossible machines before the glacier city shifts again.",
                Price = 34.99m,
                ReleaseDate = new DateOnly(2026, 6, 11),
                CoverImageUrl = "/images/games/glacier-guild.webp",
                CreatedAt = new DateTime(2026, 7, 27, 10, 0, 0, DateTimeKind.Utc)
            },
            new
            {
                Id = Guid.Parse("a55be02c-f531-472f-a763-4598b470cb10"),
                SellerId = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Slug = "midnight-parcel",
                Title = "Midnight Parcel",
                Description = "Deliver fragile packages across rain-soaked rooftops while rival couriers chase the same impossible route.",
                Price = 9.99m,
                ReleaseDate = new DateOnly(2025, 2, 25),
                CoverImageUrl = "/images/games/midnight-parcel.webp",
                CreatedAt = new DateTime(2026, 7, 27, 10, 0, 0, DateTimeKind.Utc)
            });
    }
}
