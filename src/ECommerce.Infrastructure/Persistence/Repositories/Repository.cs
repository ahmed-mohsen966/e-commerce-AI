using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    private readonly ApplicationDbContext _context;

    public Repository(ApplicationDbContext context) => _context = context;

    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _context.Set<T>().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public IQueryable<T> Query() => _context.Set<T>();

    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        _context.Set<T>().Add(entity);
        return Task.CompletedTask;
    }

    public void Update(T entity) => _context.Set<T>().Update(entity);

    public void Remove(T entity) => _context.Set<T>().Remove(entity);
}
