using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Auth.Dtos;
using MediatR;

namespace ECommerce.Application.Features.Auth.Commands;

public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IIdentityService _identityService;

    public LoginCommandHandler(IIdentityService identityService) => _identityService = identityService;

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var outcome = await _identityService.LoginAsync(request.Email, request.Password, cancellationToken);

        if (!outcome.Succeeded)
            throw new AuthenticationException("Invalid email or password.");

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
