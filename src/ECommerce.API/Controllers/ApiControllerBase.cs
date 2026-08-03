using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    private ISender? _mediator;

    protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    /// <summary>
    /// The customer profile linked to the authenticated user (from the "customerId" JWT claim).
    /// Only valid on actions restricted to the Customer role — every Customer-role account is
    /// guaranteed one by RegisterCommandHandler, so a missing/invalid claim here means the route
    /// isn't actually role-gated correctly.
    /// </summary>
    protected Guid CustomerId
    {
        get
        {
            var value = User.FindFirst("customerId")?.Value;
            return Guid.TryParse(value, out var customerId)
                ? customerId
                : throw new InvalidOperationException("The authenticated user has no associated customer profile.");
        }
    }
}
