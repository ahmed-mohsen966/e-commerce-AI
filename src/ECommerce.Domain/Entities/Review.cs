using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

public class Review : BaseEntity
{
    public Guid ProductId { get; private set; }
    public Guid CustomerId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }

    private Review() { }

    private Review(Guid productId, Guid customerId, int rating, string? comment)
    {
        ProductId = productId;
        CustomerId = customerId;
        Rating = rating;
        Comment = comment;
    }

    public static Review Create(Guid productId, Guid customerId, int rating, string? comment = null)
    {
        if (productId == Guid.Empty)
            throw new DomainException("A review must reference a product.");
        if (customerId == Guid.Empty)
            throw new DomainException("A review must reference a customer.");
        if (rating is < 1 or > 5)
            throw new DomainException("Rating must be between 1 and 5.");

        return new Review(productId, customerId, rating, comment?.Trim());
    }

    public void Edit(int rating, string? comment)
    {
        if (rating is < 1 or > 5)
            throw new DomainException("Rating must be between 1 and 5.");

        Rating = rating;
        Comment = comment?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
