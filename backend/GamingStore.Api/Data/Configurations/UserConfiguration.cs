namespace GamingStore.Api.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(user => user.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(user => user.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.HasData(
            new User
            {
                Id = Guid.Parse("6d16f5fd-0e50-4e25-894c-5f2d5a767b7f"),
                FirstName = "Alex",
                LastName = "Player",
                Email = "alex.player@gamingstore.local",
                CreatedAt = new DateTime(2026, 7, 23, 10, 0, 0, DateTimeKind.Utc)
            });
    }
}
