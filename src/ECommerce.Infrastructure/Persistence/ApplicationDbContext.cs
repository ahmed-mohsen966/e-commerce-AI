using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Common;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
{
    private readonly IPublisher? _publisher;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IPublisher? publisher = null)
        : base(options)
    {
        _publisher = publisher;
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Domain events are dispatched in-memory only; never persisted.
        modelBuilder.Ignore<BaseEvent>();

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatched BEFORE the actual commit: handler side effects (e.g. stock decrement)
        // get added to this same change tracker and are persisted atomically below, in the
        // single base.SaveChangesAsync call, alongside whatever raised the event.
        if (_publisher is not null)
            await DispatchDomainEventsAsync(cancellationToken);

        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken)
    {
        while (true)
        {
            var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
                .Select(e => e.Entity)
                .Where(e => e.DomainEvents.Count > 0)
                .ToList();

            if (entitiesWithEvents.Count == 0)
                break;

            var domainEvents = entitiesWithEvents.SelectMany(e => e.DomainEvents).ToList();

            foreach (var entity in entitiesWithEvents)
                entity.ClearDomainEvents();

            foreach (var domainEvent in domainEvents)
                await _publisher!.Publish(domainEvent, cancellationToken);
        }
    }
}
