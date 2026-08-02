using ECommerce.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ECommerce.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));

        services.AddTransient(
            typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}
