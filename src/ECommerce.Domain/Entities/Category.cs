using ECommerce.Domain.Common;

namespace ECommerce.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid? ParentCategoryId { get; private set; }

    private Category() { }

    private Category(string name, string? description, Guid? parentCategoryId)
    {
        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId;
    }

    public static Category Create(string name, string? description = null, Guid? parentCategoryId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name is required.");

        return new Category(name.Trim(), description?.Trim(), parentCategoryId);
    }

    public void UpdateDetails(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name is required.");

        Name = name.Trim();
        Description = description?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void MoveTo(Guid? parentCategoryId)
    {
        if (parentCategoryId.HasValue && parentCategoryId.Value == Id)
            throw new DomainException("A category cannot be its own parent.");

        ParentCategoryId = parentCategoryId;
        UpdatedAt = DateTime.UtcNow;
    }
}
