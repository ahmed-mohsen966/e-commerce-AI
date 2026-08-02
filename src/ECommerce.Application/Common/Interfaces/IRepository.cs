using ECommerce.Domain.Common;

namespace ECommerce.Application.Common.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    IQueryable<T> Query();

    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    void Update(T entity);

    void Remove(T entity);
}
