namespace ECommerce.Application.Features.Carts.Dtos;

public sealed class CartDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public IReadOnlyCollection<CartItemDto> Items { get; init; } = [];
    public decimal TotalAmount { get; init; }
    public string TotalCurrency { get; init; } = "USD";
}
