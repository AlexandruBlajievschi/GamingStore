using System.ComponentModel.DataAnnotations;

namespace GamingStore.Api.DTOs;

public sealed record RegisterRequest(
    [param: Required, StringLength(100)] string FirstName,
    [param: Required, StringLength(100)] string LastName,
    [param: Required, EmailAddress, StringLength(320)] string Email,
    [param: Required, StringLength(128, MinimumLength = 15)] string Password);

public sealed record LoginRequest(
    [param: Required, EmailAddress, StringLength(320)] string Email,
    [param: Required, StringLength(128)] string Password);

public sealed record AuthenticatedUserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email);

public sealed record AntiforgeryTokenResponse(string Token);
