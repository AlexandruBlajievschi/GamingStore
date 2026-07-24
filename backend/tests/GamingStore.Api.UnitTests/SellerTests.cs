using GamingStore.Api.Models;
using GamingStore.Api.Models.Entities;

namespace GamingStore.Api.UnitTests;

public sealed class SellerTests
{
    [Fact]
    public void Create_NormalizesValidValues()
    {
        var seller = Seller.Create(" Northbyte Games ", " STUDIO@NORTHBYTE.LOCAL ", " Indie studio. ");

        Assert.NotEqual(Guid.Empty, seller.Id);
        Assert.Equal("Northbyte Games", seller.Name);
        Assert.Equal("studio@northbyte.local", seller.Email);
        Assert.Equal("Indie studio.", seller.Description);
        Assert.Empty(seller.Games);
    }

    [Fact]
    public void Create_ReturnsNullDescription_WhenDescriptionIsEmpty()
    {
        var seller = Seller.Create("Northbyte Games", "studio@northbyte.local", "   ");

        Assert.Null(seller.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsValidation_WhenNameIsEmpty(string name)
    {
        Assert.Throws<DomainValidationException>(
            () => Seller.Create(name, "studio@northbyte.local"));
    }

    [Fact]
    public void Create_ThrowsValidation_WhenNameIsTooLong()
    {
        Assert.Throws<DomainValidationException>(
            () => Seller.Create(new string('A', 151), "studio@northbyte.local"));
    }

    [Fact]
    public void Create_ThrowsValidation_WhenEmailIsInvalid()
    {
        Assert.Throws<DomainValidationException>(
            () => Seller.Create("Northbyte Games", "not-an-email"));
    }

    [Fact]
    public void Create_ThrowsValidation_WhenEmailContainsDisplayName()
    {
        Assert.Throws<DomainValidationException>(
            () => Seller.Create("Northbyte Games", "Studio <studio@northbyte.local>"));
    }

    [Fact]
    public void Create_ThrowsValidation_WhenEmailIsTooLong()
    {
        var email = $"{new string('a', 309)}@example.local";

        Assert.Throws<DomainValidationException>(
            () => Seller.Create("Northbyte Games", email));
    }

    [Fact]
    public void Create_ThrowsValidation_WhenDescriptionIsTooLong()
    {
        Assert.Throws<DomainValidationException>(
            () => Seller.Create("Northbyte Games", "studio@northbyte.local", new string('A', 1001)));
    }

    [Fact]
    public void UpdateDetails_NormalizesAndUpdatesValidValues()
    {
        var seller = Seller.Create("Old Studio", "old@studio.local", "Old description.");

        seller.UpdateDetails(" New Studio ", " NEW@STUDIO.LOCAL ", " New description. ");

        Assert.Equal("New Studio", seller.Name);
        Assert.Equal("new@studio.local", seller.Email);
        Assert.Equal("New description.", seller.Description);
    }

    [Fact]
    public void UpdateDetails_ReturnsNullDescription_WhenDescriptionIsEmpty()
    {
        var seller = Seller.Create("Old Studio", "old@studio.local", "Old description.");

        seller.UpdateDetails("New Studio", "new@studio.local", "   ");

        Assert.Null(seller.Description);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_ThrowsValidation_WhenNameIsEmpty(string name)
    {
        var seller = Seller.Create("Old Studio", "old@studio.local");

        Assert.Throws<DomainValidationException>(
            () => seller.UpdateDetails(name, "new@studio.local"));
    }

    [Fact]
    public void UpdateDetails_ThrowsValidation_WhenNameIsTooLong()
    {
        var seller = Seller.Create("Old Studio", "old@studio.local");

        Assert.Throws<DomainValidationException>(
            () => seller.UpdateDetails(new string('A', 151), "new@studio.local"));
    }

    [Fact]
    public void UpdateDetails_ThrowsValidation_WhenEmailIsInvalid()
    {
        var seller = Seller.Create("Old Studio", "old@studio.local");

        Assert.Throws<DomainValidationException>(
            () => seller.UpdateDetails("New Studio", "not-an-email"));
    }

    [Fact]
    public void UpdateDetails_ThrowsValidation_WhenEmailContainsDisplayName()
    {
        var seller = Seller.Create("Old Studio", "old@studio.local");

        Assert.Throws<DomainValidationException>(
            () => seller.UpdateDetails("New Studio", "Studio <new@studio.local>"));
    }

    [Fact]
    public void UpdateDetails_ThrowsValidation_WhenEmailIsTooLong()
    {
        var seller = Seller.Create("Old Studio", "old@studio.local");
        var email = $"{new string('a', 309)}@example.local";

        Assert.Throws<DomainValidationException>(
            () => seller.UpdateDetails("New Studio", email));
    }

    [Fact]
    public void UpdateDetails_ThrowsValidation_WhenDescriptionIsTooLong()
    {
        var seller = Seller.Create("Old Studio", "old@studio.local");

        Assert.Throws<DomainValidationException>(
            () => seller.UpdateDetails("New Studio", "new@studio.local", new string('A', 1001)));
    }
}
