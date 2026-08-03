using ECommerce.Domain.Common;

namespace ECommerce.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs <paramref name="operation"/> inside a database transaction, committing on success
    /// and rolling back if it throws. Use when a unit of work spans more than one SaveChanges
    /// call (e.g. side effects triggered by a domain event handler).
    /// </summary>
    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default);
}
