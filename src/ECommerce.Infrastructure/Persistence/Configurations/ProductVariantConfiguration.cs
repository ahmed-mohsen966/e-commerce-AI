using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Infrastructure.Persistence.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Sku).IsRequired().HasMaxLength(50);
        builder.Property(v => v.Size).IsRequired().HasMaxLength(50);
        builder.Property(v => v.Color).IsRequired().HasMaxLength(50);
        builder.Property(v => v.StockQuantity).IsRequired();

        builder.OwnsOne(v => v.Price, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("PriceAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("PriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.HasIndex(v => v.Sku).IsUnique();

        // Relationship to Product (principal) is configured in ProductConfiguration.
        builder.HasIndex(v => v.ProductId);
    }
}
