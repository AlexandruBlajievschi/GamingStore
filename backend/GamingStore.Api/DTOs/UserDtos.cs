namespace GamingStore.Api.DTOs;

public sealed record CreateUserRequest(string FirstName, string LastName, string Email);

public sealed record UpdateUserRequest(string FirstName, string LastName, string Email);

public sealed record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    DateTime CreatedAt);
