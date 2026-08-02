using ECommerce.Application.Common.Constants;
using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace ECommerce.Application.Features.Users.Commands;

public sealed record PromoteToAdminCommand(string Email) : IRequest;

public sealed class PromoteToAdminCommandHandler : IRequestHandler<PromoteToAdminCommand>
{
    private readonly IIdentityService _identityService;

    public PromoteToAdminCommandHandler(IIdentityService identityService) => _identityService = identityService;

    public async Task Handle(PromoteToAdminCommand request, CancellationToken cancellationToken)
    {
        var outcome = await _identityService.AssignRoleAsync(request.Email, IdentityRoles.Admin, cancellationToken);

        if (outcome.Succeeded)
            return;

        if (outcome.Errors.Any(e => e.Code == "UserNotFound"))
            throw new NotFoundException("User", request.Email);

        throw new ValidationException(outcome.Errors.Select(e =>
            new ValidationFailure(nameof(PromoteToAdminCommand.Email), e.Description)));
    }
}
