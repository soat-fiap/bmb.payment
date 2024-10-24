using System.Diagnostics.CodeAnalysis;
using Bmb.Payment.Core.Contracts;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Payment.Bus;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddPaymentBus(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<OrderCreatedConsumer>();
            x.UsingAmazonSqs((context, cfg) =>
            {
                cfg.Host("us-east-1", h =>
                {
                    // h.Scope("dev", true);
                });
                cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter( false));
            });
        });

        services.AddScoped<IDispatcher, Dispatcher>();
    }
}