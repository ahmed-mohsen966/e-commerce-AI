using Asp.Versioning;
using ECommerce.Application.Common.Constants;
using ECommerce.Application.Features.Carts.Commands;
using ECommerce.Application.Features.Carts.Dtos;
using ECommerce.Application.Features.Carts.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.V1;

/// <summary>
/// The authenticated customer's shopping cart.
/// </summary>
[ApiVersion("1.0")]
[Authorize(Roles = IdentityRoles.Customer)]
public sealed class CartController : ApiControllerBase
{
    /// <summary>
    /// Gets the current customer's cart. Returns an empty cart if nothing has been added yet.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDto>> Get(CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetCartQuery(CustomerId), cancellationToken));
    }

    /// <summary>
    /// Adds an item to the current customer's cart, merging with an existing line for the same variant.
    /// </summary>
    /// <param name="request">The variant and quantity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartDto>> AddItem(AddCartItemRequest request, CancellationToken cancellationToken)
    {
        var command = new AddToCartCommand(CustomerId, request.ProductVariantId, request.Quantity);
        return Ok(await Mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// Sets the quantity of an existing cart line.
    /// </summary>
    /// <param name="productVariantId">The variant whose line to update.</param>
    /// <param name="request">The new quantity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("items/{productVariantId:guid}")]
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartDto>> UpdateItem(
        Guid productVariantId, UpdateCartItemRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateCartItemCommand(CustomerId, productVariantId, request.Quantity);
        return Ok(await Mediator.Send(command, cancellationToken));
    }

    /// <summary>
    /// Removes a line from the current customer's cart.
    /// </summary>
    /// <param name="productVariantId">The variant whose line to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("items/{productVariantId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(Guid productVariantId, CancellationToken cancellationToken)
    {
        await Mediator.Send(new RemoveFromCartCommand(CustomerId, productVariantId), cancellationToken);
        return NoContent();
    }
}

public sealed record AddCartItemRequest(Guid ProductVariantId, int Quantity);

public sealed record UpdateCartItemRequest(int Quantity);
