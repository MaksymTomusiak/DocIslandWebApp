namespace Application.Common.Interfaces.Services.Providers;

public interface ICurrentUserProvider
{
    string? ClerkUserId { get; }
    string? Email { get; }
}