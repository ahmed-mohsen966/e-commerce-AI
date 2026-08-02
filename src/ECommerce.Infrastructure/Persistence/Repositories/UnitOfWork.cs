using System.Collections.Concurrent;
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    public UnitOfWork(ApplicationDbContext context) => _context = context;

    public IRepository<T> Repository<T>() where T : BaseEntity =>
        (IRepository<T>)_repositories.GetOrAdd(typeof(T), _ => new Repository<T>(_context));

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _context.SaveChangesAsync(cancellationToken);
}
