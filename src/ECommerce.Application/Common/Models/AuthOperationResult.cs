namespace ECommerce.Application.Common.Models;

public sealed record AuthResult(
    string AccessToken,
    DateTime AccessTokenExpiresAtUtc,
    string RefreshToken,
    DateTime RefreshTokenExpiresAtUtc);

public sealed record IdentityOperationError(string Code, string Description);

public sealed class AuthOperationResult
{
    public bool Succeeded { get; private init; }
    public AuthResult? Auth { get; private init; }
    public IReadOnlyCollection<IdentityOperationError> Errors { get; private init; } = [];

    public static AuthOperationResult Success(AuthResult auth) => new() { Succeeded = true, Auth = auth };

    public static AuthOperationResult Failure(params IdentityOperationError[] errors) =>
        new() { Succeeded = false, Errors = errors };

    public static AuthOperationResult Failure(IEnumerable<IdentityOperationError> errors) =>
        new() { Succeeded = false, Errors = errors.ToArray() };
}

public sealed class IdentityOperationOutcome
{
    public bool Succeeded { get; private init; }
    public IReadOnlyCollection<IdentityOperationError> Errors { get; private init; } = [];

    public static IdentityOperationOutcome Success() => new() { Succeeded = true };

    public static IdentityOperationOutcome Failure(params IdentityOperationError[] errors) =>
        new() { Succeeded = false, Errors = errors };

    public static IdentityOperationOutcome Failure(IEnumerable<IdentityOperationError> errors) =>
        new() { Succeeded = false, Errors = errors.ToArray() };
}
