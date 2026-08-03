using Asp.Versioning;
using ECommerce.Application.Common.Constants;
using ECommerce.Application.Features.Addresses.Commands;
using ECommerce.Application.Features.Addresses.Dtos;
using ECommerce.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.V1;

/// <summary>
/// Addresses belonging to the authenticated customer.
/// </summary>
[ApiVersion("1.0")]
[Authorize(Roles = IdentityRoles.Customer)]
public sealed class AddressesController : ApiControllerBase
{
    /// <summary>
    /// Adds a new address to the current customer's profile.
    /// </summary>
    /// <param name="request">The address details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AddressDto>> Add(AddAddressRequest request, CancellationToken cancellationToken)
    {
        var command = new AddAddressCommand(
            CustomerId,
            request.Type,
            request.Line1,
            request.Line2,
            request.City,
            request.State,
            request.PostalCode,
            request.Country,
            request.IsDefault);

        return Ok(await Mediator.Send(command, cancellationToken));
    }
}

public sealed record AddAddressRequest(
    AddressType Type,
    string Line1,
    string? Line2,
    string City,
    string State,
    string PostalCode,
    string Country,
    bool IsDefault = false);
