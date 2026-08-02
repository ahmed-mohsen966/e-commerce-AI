using Asp.Versioning;
using ECommerce.Application.Features.Categories.Commands;
using ECommerce.Application.Features.Categories.Dtos;
using ECommerce.Application.Features.Categories.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers.V1;

/// <summary>
/// Manages product categories.
/// </summary>
[ApiVersion("1.0")]
public sealed class CategoriesController : ApiControllerBase
{
    /// <summary>
    /// Gets all categories, ordered by name.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<CategoryDto>>> GetList(CancellationToken cancellationToken)
    {
        return Ok(await Mediator.Send(new GetCategoriesQuery(), cancellationToken));
    }

    /// <summary>
    /// Creates a new category, optionally nested under a parent category.
    /// </summary>
    /// <param name="command">The category to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Guid>> Create(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var id = await Mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, id);
    }
}
