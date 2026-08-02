using ECommerce.Domain.Common;

namespace ECommerce.Domain.ValueObjects;

public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency = "USD")
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative.");
        if (string.IsNullOrWhiteSpace(currency) || currency.Trim().Length != 3)
            throw new DomainException("Currency must be a 3-letter ISO code.");

        return new Money(amount, currency.Trim().ToUpperInvariant());
    }

    public static Money Zero(string currency = "USD") => new(0m, currency.Trim().ToUpperInvariant());

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);
        var result = Amount - other.Amount;
        if (result < 0)
            throw new DomainException("Resulting amount cannot be negative.");

        return new Money(result, Currency);
    }

    public Money Multiply(int factor)
    {
        if (factor < 0)
            throw new DomainException("Factor cannot be negative.");

        return new Money(Amount * factor, Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot operate on Money with different currencies: {Currency} vs {other.Currency}.");
    }

    public static Money operator +(Money left, Money right) => left.Add(right);
    public static Money operator -(Money left, Money right) => left.Subtract(right);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Amount:0.00} {Currency}";
}
