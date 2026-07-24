namespace GamingStore.Api.Data.Configurations;

public sealed class SellerConfiguration : IEntityTypeConfiguration<Seller>
{
    public void Configure(EntityTypeBuilder<Seller> builder)
    {
        builder.ToTable("Sellers");

        builder.HasKey(seller => seller.Id);

        builder.Property(seller => seller.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(seller => seller.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(seller => seller.Description)
            .HasMaxLength(1000);

        builder.HasIndex(seller => seller.Email)
            .IsUnique();

        builder.Property(seller => seller.CreatedAt)
            .IsRequired();

        builder.HasData(
            new
            {
                Id = Guid.Parse("84aa2fc0-1089-47d2-8a50-82f4d6e1de5f"),
                Name = "Northbyte Games",
                Email = "studio@northbyte.local",
                Description = "Independent seller focused on thoughtful PC games.",
                CreatedAt = new DateTime(2026, 7, 23, 10, 0, 0, DateTimeKind.Utc)
            });
    }
}
