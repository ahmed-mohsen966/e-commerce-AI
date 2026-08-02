using Asp.Versioning;
using ECommerce.Application.Common.Constants;
using ECommerce.Application.Features.Users.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.V1;

/// <summary>
/// Admin-only user management.
/// </summary>
[ApiVersion("1.0")]
[Authorize(Roles = IdentityRoles.Admin)]
public sealed class UsersController : ApiControllerBase
{
    /// <summary>
    /// Promotes an existing user (by email) to the Admin role. Idempotent if already an admin.
    /// </summary>
    /// <param name="command">The email of the user to promote.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("promote-to-admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PromoteToAdmin(PromoteToAdminCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
