using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.Events;
using Bmb.Payment.Core.Contracts;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bmb.Payment.Bus;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddPaymentBus(this IServiceCollection services)
    {
        services.AddMassTransit(bus =>
        {
            bus.AddConsumer<OrderCreatedPaymentConsumer>();
            bus.UsingAmazonSqs((context, cfg) =>
            {
                cfg.Host("us-east-1", h =>
                {
                    // h.Scope("dev", true);
                });
                cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter( false));
            });
            
            bus.ConfigureHealthCheckOptions(options =>
            {
                options.Name = "masstransit";
                options.MinimalFailureStatus = HealthStatus.Unhealthy;
                options.Tags.Add("health");
            });
        });

        services.AddScoped<IDispatcher, Dispatcher>();
    }
}