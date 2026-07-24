namespace GamingStore.Api.Models;

public sealed class ResourceNotFoundException(string message) : Exception(message);
