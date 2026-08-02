using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

public class Address : BaseEntity
{
    public Guid CustomerId { get; private set; }
    public AddressType Type { get; private set; }
    public string Line1 { get; private set; } = null!;
    public string? Line2 { get; private set; }
    public string City { get; private set; } = null!;
    public string State { get; private set; } = null!;
    public string PostalCode { get; private set; } = null!;
    public string Country { get; private set; } = null!;
    public bool IsDefault { get; private set; }

    private Address() { }

    internal Address(
        Guid customerId,
        AddressType type,
        string line1,
        string? line2,
        string city,
        string state,
        string postalCode,
        string country,
        bool isDefault)
    {
        if (customerId == Guid.Empty)
            throw new DomainException("An address must belong to a customer.");
        if (string.IsNullOrWhiteSpace(line1))
            throw new DomainException("Address line 1 is required.");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City is required.");
        if (string.IsNullOrWhiteSpace(state))
            throw new DomainException("State is required.");
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new DomainException("Postal code is required.");
        if (string.IsNullOrWhiteSpace(country))
            throw new DomainException("Country is required.");

        CustomerId = customerId;
        Type = type;
        Line1 = line1.Trim();
        Line2 = line2?.Trim();
        City = city.Trim();
        State = state.Trim();
        PostalCode = postalCode.Trim();
        Country = country.Trim();
        IsDefault = isDefault;
    }

    public void UpdateDetails(string line1, string? line2, string city, string state, string postalCode, string country)
    {
        if (string.IsNullOrWhiteSpace(line1))
            throw new DomainException("Address line 1 is required.");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City is required.");
        if (string.IsNullOrWhiteSpace(state))
            throw new DomainException("State is required.");
        if (string.IsNullOrWhiteSpace(postalCode))
            throw new DomainException("Postal code is required.");
        if (string.IsNullOrWhiteSpace(country))
            throw new DomainException("Country is required.");

        Line1 = line1.Trim();
        Line2 = line2?.Trim();
        City = city.Trim();
        State = state.Trim();
        PostalCode = postalCode.Trim();
        Country = country.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    internal void SetAsDefault()
    {
        IsDefault = true;
        UpdatedAt = DateTime.UtcNow;
    }

    internal void UnsetDefault()
    {
        IsDefault = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
