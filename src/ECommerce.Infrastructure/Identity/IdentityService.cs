using ECommerce.Application.Common.Constants;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Common.Models;
using ECommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ECommerce.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly JwtSettings _jwtSettings;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ApplicationDbContext context,
        IJwtTokenGenerator tokenGenerator,
        IOptions<JwtSettings> jwtOptions)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _tokenGenerator = tokenGenerator;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthOperationResult> RegisterAsync(
        string email, string password, Guid customerId, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            CustomerId = customerId,
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            return AuthOperationResult.Failure(
                createResult.Errors.Select(e => new IdentityOperationError(e.Code, e.Description)));
        }

        await _userManager.AddToRoleAsync(user, IdentityRoles.Customer);

        var auth = await IssueTokensAsync(user, cancellationToken);
        return AuthOperationResult.Success(auth);
    }

    public async Task<AuthOperationResult> LoginAsync(
        string email, string password, CancellationToken cancellationToken = default)
    {
        var invalidCredentials = AuthOperationResult.Failure(
            new IdentityOperationError("InvalidCredentials", "Invalid email or password."));

        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
            return invalidCredentials;

        var signInResult = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (!signInResult.Succeeded)
            return invalidCredentials;

        var auth = await IssueTokensAsync(user, cancellationToken);
        return AuthOperationResult.Success(auth);
    }

    public async Task<AuthOperationResult> RefreshTokenAsync(
        string refreshToken, CancellationToken cancellationToken = default)
    {
        var invalidToken = AuthOperationResult.Failure(
            new IdentityOperationError("InvalidToken", "Invalid or expired refresh token."));

        var existing = await _context.Set<RefreshToken>()
            .FirstOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);

        if (existing is null || !existing.IsActive)
            return invalidToken;

        var user = await _userManager.FindByIdAsync(existing.UserId.ToString());
        if (user is null)
            return invalidToken;

        existing.RevokedAtUtc = DateTime.UtcNow;

        var auth = await IssueTokensAsync(user, cancellationToken, existing);
        return AuthOperationResult.Success(auth);
    }

    public async Task<IdentityOperationOutcome> AssignRoleAsync(
        string email, string role, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return IdentityOperationOutcome.Failure(
                new IdentityOperationError("UserNotFound", $"No user found with email '{email}'."));
        }

        if (await _userManager.IsInRoleAsync(user, role))
            return IdentityOperationOutcome.Success();

        var result = await _userManager.AddToRoleAsync(user, role);
        if (!result.Succeeded)
        {
            return IdentityOperationOutcome.Failure(
                result.Errors.Select(e => new IdentityOperationError(e.Code, e.Description)));
        }

        return IdentityOperationOutcome.Success();
    }

    private async Task<AuthResult> IssueTokensAsync(
        ApplicationUser user, CancellationToken cancellationToken, RefreshToken? replacing = null)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var (accessToken, accessExpiresAtUtc) = _tokenGenerator.GenerateAccessToken(user, roles);

        var refreshTokenValue = _tokenGenerator.GenerateRefreshToken();
        var refreshExpiresAtUtc = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);

        var refreshTokenEntity = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAtUtc = refreshExpiresAtUtc,
        };

        if (replacing is not null)
            replacing.ReplacedByToken = refreshTokenValue;

        _context.Set<RefreshToken>().Add(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResult(accessToken, accessExpiresAtUtc, refreshTokenValue, refreshExpiresAtUtc);
    }
}
