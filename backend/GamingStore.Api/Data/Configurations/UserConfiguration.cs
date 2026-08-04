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

        builder.Property(user => user.UserName)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(user => user.NormalizedUserName)
            .HasMaxLength(320);

        builder.Property(user => user.NormalizedEmail)
            .HasMaxLength(320);

        builder.HasIndex(user => user.NormalizedEmail)
            .IsUnique()
            .HasDatabaseName("EmailIndex");

        builder.Property(user => user.CreatedAt)
            .IsRequired();

        builder.HasData(
            new
            {
                Id = Guid.Parse("6d16f5fd-0e50-4e25-894c-5f2d5a767b7f"),
                AccessFailedCount = 0,
                ConcurrencyStamp = "6a449dac-52cc-4d02-8581-c800bc5d6453",
                CreatedAt = new DateTime(2026, 7, 23, 10, 0, 0, DateTimeKind.Utc),
                Email = "alex.player@gamingstore.local",
                EmailConfirmed = true,
                FirstName = "Alex",
                LastName = "Player",
                LockoutEnabled = true,
                LockoutEnd = (DateTimeOffset?)null,
                NormalizedEmail = "ALEX.PLAYER@GAMINGSTORE.LOCAL",
                NormalizedUserName = "ALEX.PLAYER@GAMINGSTORE.LOCAL",
                PasswordHash = (string?)null,
                PhoneNumber = (string?)null,
                PhoneNumberConfirmed = false,
                SecurityStamp = "7b1b31d7-ff8c-4eb9-aed3-0d205b169807",
                TwoFactorEnabled = false,
                UserName = "alex.player@gamingstore.local"
            });
    }
}
