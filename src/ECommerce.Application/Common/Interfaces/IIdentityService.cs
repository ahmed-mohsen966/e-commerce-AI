using ECommerce.Application.Common.Models;

namespace ECommerce.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<AuthOperationResult> RegisterAsync(
        string email, string password, Guid customerId, CancellationToken cancellationToken = default);

    Task<AuthOperationResult> LoginAsync(
        string email, string password, CancellationToken cancellationToken = default);

    Task<AuthOperationResult> RefreshTokenAsync(
        string refreshToken, CancellationToken cancellationToken = default);

    Task<IdentityOperationOutcome> AssignRoleAsync(
        string email, string role, CancellationToken cancellationToken = default);
}
