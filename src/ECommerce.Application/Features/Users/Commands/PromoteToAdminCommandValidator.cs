using FluentValidation;

namespace ECommerce.Application.Features.Users.Commands;

public sealed class PromoteToAdminCommandValidator : AbstractValidator<PromoteToAdminCommand>
{
    public PromoteToAdminCommandValidator() => RuleFor(x => x.Email).NotEmpty().EmailAddress();
}
