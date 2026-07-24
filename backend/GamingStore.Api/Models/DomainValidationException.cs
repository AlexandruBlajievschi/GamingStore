namespace GamingStore.Api.Models;

public sealed class DomainValidationException(string message) : Exception(message);
