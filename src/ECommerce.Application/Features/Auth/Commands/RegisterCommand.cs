using ECommerce.Application.Common.Interfaces;
using ECommerce.Application.Features.Auth.Dtos;
using ECommerce.Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace ECommerce.Application.Features.Auth.Commands;

public sealed record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<AuthResponseDto>;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var customer = Customer.Register(request.FirstName, request.LastName, request.Email);
        await _unitOfWork.Repository<Customer>().AddAsync(customer, cancellationToken);

        var outcome = await _identityService.RegisterAsync(
            request.Email, request.Password, customer.Id, cancellationToken);

        if (!outcome.Succeeded)
        {
            throw new ValidationException(outcome.Errors.Select(e =>
                new ValidationFailure(MapField(e.Code), e.Description)));
        }

        var auth = outcome.Auth!;
        return new AuthResponseDto
        {
            AccessToken = auth.AccessToken,
            AccessTokenExpiresAtUtc = auth.AccessTokenExpiresAtUtc,
            RefreshToken = auth.RefreshToken,
            RefreshTokenExpiresAtUtc = auth.RefreshTokenExpiresAtUtc,
        };
    }

    private static string MapField(string errorCode) => errorCode switch
    {
        var c when c.Contains("Password", StringComparison.OrdinalIgnoreCase) => nameof(RegisterCommand.Password),
        var c when c.Contains("Email", StringComparison.OrdinalIgnoreCase)
            || c.Contains("UserName", StringComparison.OrdinalIgnoreCase) => nameof(RegisterCommand.Email),
        _ => string.Empty,
    };
}
