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
            });
    }
}
