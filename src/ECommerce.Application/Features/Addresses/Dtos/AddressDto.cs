using ECommerce.Domain.Enums;

namespace ECommerce.Application.Features.Addresses.Dtos;

public sealed class AddressDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public AddressType Type { get; init; }
    public string Line1 { get; init; } = null!;
    public string? Line2 { get; init; }
    public string City { get; init; } = null!;
    public string State { get; init; } = null!;
    public string PostalCode { get; init; } = null!;
    public string Country { get; init; } = null!;
    public bool IsDefault { get; init; }
}
