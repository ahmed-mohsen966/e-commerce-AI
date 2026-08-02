using ECommerce.Domain.Common;
using ECommerce.Domain.Enums;
using ECommerce.Domain.ValueObjects;

namespace ECommerce.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Money Amount { get; private set; } = null!;
    public PaymentMethod Method { get; private set; }
    public PaymentStatus Status { get; private set; }
    public string? TransactionId { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public string? FailureReason { get; private set; }

    private Payment() { }

    private Payment(Guid orderId, Money amount, PaymentMethod method)
    {
        OrderId = orderId;
        Amount = amount;
        Method = method;
        Status = PaymentStatus.Pending;
    }

    public static Payment Initiate(Guid orderId, Money amount, PaymentMethod method)
    {
        if (orderId == Guid.Empty)
            throw new DomainException("A payment must reference an order.");
        if (amount.Amount <= 0)
            throw new DomainException("Payment amount must be greater than zero.");

        return new Payment(orderId, amount, method);
    }

    public void Complete(string transactionId)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException($"Cannot complete a payment in '{Status}' status.");
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new DomainException("A transaction id is required to complete a payment.");

        Status = PaymentStatus.Completed;
        TransactionId = transactionId.Trim();
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Fail(string reason)
    {
        if (Status != PaymentStatus.Pending)
            throw new DomainException($"Cannot fail a payment in '{Status}' status.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("A failure reason is required.");

        Status = PaymentStatus.Failed;
        FailureReason = reason.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Refund()
    {
        if (Status != PaymentStatus.Completed)
            throw new DomainException("Only completed payments can be refunded.");

        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTime.UtcNow;
    }
}
