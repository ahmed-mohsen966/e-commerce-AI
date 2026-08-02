using ECommerce.Domain.Common;

namespace ECommerce.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
