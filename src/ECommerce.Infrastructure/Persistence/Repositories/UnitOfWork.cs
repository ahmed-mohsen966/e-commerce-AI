using System.Collections.Concurrent;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public IRepository<T> Repository<T>() where T : BaseEntity =>
        (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new Repository<T>(context));

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
    {
        var strategy = context.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await operation(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}
