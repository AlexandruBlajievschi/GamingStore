using GamingStore.Api.Models;
using GamingStore.Api.Models.Entities;

namespace GamingStore.Api.UnitTests;

public sealed class UserTests
{
    [Fact]
    public void Create_NormalizesValidValues()
    {
        var user = User.Create(" Alex ", " Player ", " ALEX.PLAYER@GAMINGSTORE.LOCAL ");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Alex", user.FirstName);
        Assert.Equal("Player", user.LastName);
        Assert.Equal("alex.player@gamingstore.local", user.Email);
        Assert.Equal("alex.player@gamingstore.local", user.UserName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsValidation_WhenFirstNameIsEmpty(string firstName)
    {
        Assert.Throws<DomainValidationException>(
            () => User.Create(firstName, "Player", "alex.player@gamingstore.local"));
    }

    [Fact]
    public void Create_ThrowsValidation_WhenFirstNameIsTooLong()
    {
        Assert.Throws<DomainValidationException>(
            () => User.Create(new string('A', 101), "Player", "alex.player@gamingstore.local"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_ThrowsValidation_WhenLastNameIsEmpty(string lastName)
    {
        Assert.Throws<DomainValidationException>(
            () => User.Create("Alex", lastName, "alex.player@gamingstore.local"));
    }

    [Fact]
    public void Create_ThrowsValidation_WhenLastNameIsTooLong()
    {
        Assert.Throws<DomainValidationException>(
            () => User.Create("Alex", new string('A', 101), "alex.player@gamingstore.local"));
    }

    [Fact]
    public void Create_ThrowsValidation_WhenEmailIsInvalid()
    {
        Assert.Throws<DomainValidationException>(
            () => User.Create("Alex", "Player", "not-an-email"));
    }

    [Fact]
    public void Create_ThrowsValidation_WhenEmailContainsDisplayName()
    {
        Assert.Throws<DomainValidationException>(
            () => User.Create("Alex", "Player", "Alex Player <alex.player@gamingstore.local>"));
    }

    [Fact]
    public void Create_ThrowsValidation_WhenEmailIsTooLong()
    {
        var email = $"{new string('a', 309)}@example.local";

        Assert.Throws<DomainValidationException>(
            () => User.Create("Alex", "Player", email));
    }

    [Fact]
    public void UpdateDetails_NormalizesAndUpdatesValidValues()
    {
        var user = User.Create("Alex", "Player", "alex.player@gamingstore.local");

        user.UpdateDetails(" Morgan ", " Buyer ", " MORGAN.BUYER@GAMINGSTORE.LOCAL ");

        Assert.Equal("Morgan", user.FirstName);
        Assert.Equal("Buyer", user.LastName);
        Assert.Equal("morgan.buyer@gamingstore.local", user.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_ThrowsValidation_WhenFirstNameIsEmpty(string firstName)
    {
        var user = User.Create("Alex", "Player", "alex.player@gamingstore.local");

        Assert.Throws<DomainValidationException>(
            () => user.UpdateDetails(firstName, "Buyer", "morgan.buyer@gamingstore.local"));
    }

    [Fact]
    public void UpdateDetails_ThrowsValidation_WhenFirstNameIsTooLong()
    {
        var user = User.Create("Alex", "Player", "alex.player@gamingstore.local");

        Assert.Throws<DomainValidationException>(
            () => user.UpdateDetails(new string('A', 101), "Buyer", "morgan.buyer@gamingstore.local"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateDetails_ThrowsValidation_WhenLastNameIsEmpty(string lastName)
    {
        var user = User.Create("Alex", "Player", "alex.player@gamingstore.local");

        Assert.Throws<DomainValidationException>(
            () => user.UpdateDetails("Morgan", lastName, "morgan.buyer@gamingstore.local"));
    }

    [Fact]
    public void UpdateDetails_ThrowsValidation_WhenLastNameIsTooLong()
    {
        var user = User.Create("Alex", "Player", "alex.player@gamingstore.local");

        Assert.Throws<DomainValidationException>(
            () => user.UpdateDetails("Morgan", new string('A', 101), "morgan.buyer@gamingstore.local"));
    }

    [Fact]
    public void UpdateDetails_ThrowsValidation_WhenEmailIsInvalid()
    {
        var user = User.Create("Alex", "Player", "alex.player@gamingstore.local");

        Assert.Throws<DomainValidationException>(
            () => user.UpdateDetails("Morgan", "Buyer", "not-an-email"));
    }

    [Fact]
    public void UpdateDetails_ThrowsValidation_WhenEmailContainsDisplayName()
    {
        var user = User.Create("Alex", "Player", "alex.player@gamingstore.local");

        Assert.Throws<DomainValidationException>(
            () => user.UpdateDetails("Morgan", "Buyer", "Morgan Buyer <morgan.buyer@gamingstore.local>"));
    }

    [Fact]
    public void UpdateDetails_ThrowsValidation_WhenEmailIsTooLong()
    {
        var user = User.Create("Alex", "Player", "alex.player@gamingstore.local");
        var email = $"{new string('a', 309)}@example.local";

        Assert.Throws<DomainValidationException>(
            () => user.UpdateDetails("Morgan", "Buyer", email));
    }
}
