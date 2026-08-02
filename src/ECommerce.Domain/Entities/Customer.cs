using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

public class Customer : BaseEntity
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public string? PhoneNumber { get; private set; }
    public bool IsActive { get; private set; } = true;

    private readonly List<Address> _addresses = [];
    public IReadOnlyCollection<Address> Addresses => _addresses.AsReadOnly();

    public string FullName => $"{FirstName} {LastName}";

    private Customer() { }

    private Customer(string firstName, string lastName, string email, string? phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public static Customer Register(string firstName, string lastName, string email, string? phoneNumber = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");
        if (!IsValidEmail(email))
            throw new DomainException("A valid email address is required.");

        return new Customer(firstName.Trim(), lastName.Trim(), email.Trim().ToLowerInvariant(), phoneNumber?.Trim());
    }

    public Address AddAddress(
        AddressType type,
        string line1,
        string? line2,
        string city,
        string state,
        string postalCode,
        string country,
        bool isDefault = false)
    {
        var address = new Address(Id, type, line1, line2, city, state, postalCode, country, isDefault);

        if (isDefault)
        {
            foreach (var existing in _addresses.Where(a => a.Type == type))
                existing.UnsetDefault();
        }

        _addresses.Add(address);
        UpdatedAt = DateTime.UtcNow;
        return address;
    }

    public void RemoveAddress(Guid addressId)
    {
        var address = _addresses.FirstOrDefault(a => a.Id == addressId)
            ?? throw new DomainException("Address not found for this customer.");

        _addresses.Remove(address);
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateProfile(string firstName, string lastName, string? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("First name is required.");
        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("Last name is required.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        PhoneNumber = phoneNumber?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    private static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            _ = new System.Net.Mail.MailAddress(email);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
