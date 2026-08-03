using Asp.Versioning;
using ECommerce.Application.Common.Constants;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Orders.Commands;
using ECommerce.Application.Features.Orders.Dtos;
using ECommerce.Application.Features.Orders.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.V1;

/// <summary>
/// Order placement and management.
/// </summary>
[ApiVersion("1.0")]
public sealed class OrdersController : ApiControllerBase
{
    /// <summary>
    /// Places an order from the current customer's cart. Validates stock, snapshots current
    /// prices, and clears the cart.
    /// </summary>
    /// <param name="request">The shipping and billing addresses to use.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    [Authorize(Roles = IdentityRoles.Customer)]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> Place(PlaceOrderRequest request, CancellationToken cancellationToken)
    {
        var command = new PlaceOrderCommand(CustomerId, request.ShippingAddressId, request.BillingAddressId);
        var id = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Gets the current customer's orders, most recent first.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    [Authorize(Roles = IdentityRoles.Customer)]
    [ProducesResponseType(typeof(IReadOnlyCollection<OrderSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<OrderSummaryDto>>> GetMine(CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetOrdersForCustomerQuery(CustomerId), cancellationToken));
    }

    /// <summary>
    /// Gets a single order belonging to the current customer.
    /// </summary>
    /// <param name="id">The order id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = IdentityRoles.Customer)]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetOrderByIdQuery(id, CustomerId), cancellationToken));
    }

    /// <summary>
    /// Gets a paginated list of all orders, optionally filtered by status. Admin only.
    /// </summary>
    /// <param name="query">Pagination and status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("all")]
    [Authorize(Roles = IdentityRoles.Admin)]
    [ProducesResponseType(typeof(PaginatedList<OrderSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PaginatedList<OrderSummaryDto>>> GetAll(
        [FromQuery] GetAllOrdersQuery query, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// Updates an order's status (e.g. mark paid, ship, deliver, cancel). Admin only.
    /// </summary>
    /// <param name="id">The order id from the route.</param>
    /// <param name="command">The status transition to apply.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = IdentityRoles.Admin)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        Guid id, UpdateOrderStatusCommand command, CancellationToken cancellationToken)
    {
        if (id != command.OrderId)
            return BadRequest("The id in the route does not match the id in the request body.");

        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }
}

public sealed record PlaceOrderRequest(Guid ShippingAddressId, Guid BillingAddressId);
