namespace GamingStore.Api.Models;

public sealed class AuthenticationFailedException(string message) : Exception(message);
