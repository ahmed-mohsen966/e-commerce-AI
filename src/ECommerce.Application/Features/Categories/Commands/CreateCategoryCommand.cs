using ECommerce.Application.Common.Exceptions;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Features.Categories.Commands;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description,
    Guid? ParentCategoryId) : IRequest<Guid>;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (request.ParentCategoryId.HasValue)
        {
            var parentExists = await _unitOfWork.Repository<Category>()
                .Query()
                .AnyAsync(c => c.Id == request.ParentCategoryId.Value, cancellationToken);

            if (!parentExists)
                throw new NotFoundException(nameof(Category), request.ParentCategoryId.Value);
        }

        var category = Category.Create(request.Name, request.Description, request.ParentCategoryId);

        await _unitOfWork.Repository<Category>().AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return category.Id;
    }
}
