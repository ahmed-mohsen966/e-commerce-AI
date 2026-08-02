namespace ECommerce.Infrastructure.Identity;

public interface IJwtTokenGenerator
{
    (string Token, DateTime ExpiresAtUtc) GenerateAccessToken(ApplicationUser user, IList<string> roles);

    string GenerateRefreshToken();
}
