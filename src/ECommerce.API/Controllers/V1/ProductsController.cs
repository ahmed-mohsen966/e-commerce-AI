using Asp.Versioning;
using ECommerce.Application.Common.Models;
using ECommerce.Application.Features.Products.Commands;
using ECommerce.Application.Features.Products.Dtos;
using ECommerce.Application.Features.Products.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.V1;

/// <summary>
/// Manages product catalog entries.
/// </summary>
[ApiVersion("1.0")]
public sealed class ProductsController : ApiControllerBase
{
    /// <summary>
    /// Gets a paginated list of products, optionally filtered by category or price range and sorted.
    /// </summary>
    /// <param name="query">Pagination, filtering, and sorting parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<ProductListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedList<ProductListItemDto>>> GetList(
        [FromQuery] GetProductsListQuery query,
        CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(query, cancellationToken));
    }

    /// <summary>
    /// Gets a single product, including its variants, by id.
    /// </summary>
    /// <param name="id">The product id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetProductByIdQuery(id), cancellationToken));
    }

    /// <summary>
    /// Creates a new product.
    /// </summary>
    /// <param name="command">The product to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> Create(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var id = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    /// <summary>
    /// Updates an existing product.
    /// </summary>
    /// <param name="id">The product id from the route.</param>
    /// <param name="command">The updated product data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("The id in the route does not match the id in the request body.");

        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Deletes a product.
    /// </summary>
    /// <param name="id">The product id.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return NoContent();
    }
}
