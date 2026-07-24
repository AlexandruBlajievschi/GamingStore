namespace GamingStore.Api.Services;

public interface IUserService
{
    Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken);

    Task<UserResponse> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public sealed class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);

        return users
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<UserResponse> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await GetExistingUserAsync(id, trackChanges: false, cancellationToken);

        return MapToResponse(user);
    }

    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        var user = User.Create(request.FirstName, request.LastName, request.Email);

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(user);
    }

    public async Task<UserResponse> UpdateAsync(
        Guid id,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await GetExistingUserAsync(id, trackChanges: true, cancellationToken);

        user.UpdateDetails(request.FirstName, request.LastName, request.Email);
        await userRepository.SaveChangesAsync(cancellationToken);

        return MapToResponse(user);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await GetExistingUserAsync(id, trackChanges: true, cancellationToken);

        userRepository.Delete(user);
        await userRepository.SaveChangesAsync(cancellationToken);
    }

    private async Task<User> GetExistingUserAsync(
        Guid id,
        bool trackChanges,
        CancellationToken cancellationToken)
    {
        var user = trackChanges
            ? await userRepository.GetTrackedByIdAsync(id, cancellationToken)
            : await userRepository.GetByIdAsync(id, cancellationToken);

        return user ?? throw new ResourceNotFoundException($"User '{id}' was not found.");
    }

    private static UserResponse MapToResponse(User user)
    {
        return new UserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.CreatedAt);
    }
}

public sealed record CreateUserRequest(string FirstName, string LastName, string Email);

public sealed record UpdateUserRequest(string FirstName, string LastName, string Email);

public sealed record UserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    DateTime CreatedAt);
