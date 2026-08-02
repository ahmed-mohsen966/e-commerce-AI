using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Auth.Dtos;
using MediatR;

namespace ECommerce.Application.Features.Auth.Commands;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<AuthResponseDto>;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;

    public RefreshTokenCommandHandler(IIdentityService identityService) => _identityService = identityService;

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var outcome = await _identityService.RefreshTokenAsync(request.RefreshToken, cancellationToken);

        if (!outcome.Succeeded)
            throw new AuthenticationException("Invalid or expired refresh token.");

        var auth = outcome.Auth!;
        return new AuthResponseDto
        {
            AccessToken = auth.AccessToken,
            AccessTokenExpiresAtUtc = auth.AccessTokenExpiresAtUtc,
            RefreshToken = auth.RefreshToken,
            RefreshTokenExpiresAtUtc = auth.RefreshTokenExpiresAtUtc,
        };
    }
}
