using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.Interfaces;
using Bmb.Orders.Gateway;
using Bmb.Payment.Application;
using Bmb.Payment.Controllers;
using Bmb.Payment.FakePayment.Gateway;
using Bmb.Payment.MercadoPago.Gateway;
using Bmb.Payment.MySql;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Payment.DI;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void IoCSetup(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.ConfigurePersistenceApp(configuration);
        ConfigurePaymentGateway(serviceCollection);
        ConfigHybridCache(serviceCollection, configuration);
        serviceCollection.AddUseCases();
        serviceCollection.AddControllers();
        serviceCollection.AddOrdersGateway();
    }

    private static void ConfigHybridCache(IServiceCollection services, IConfiguration configuration)
    {
        var hybridCacheSettings = configuration.GetSection("HybridCache")
            .Get<HybridCacheEntryOptions>();
        services.AddHybridCache(options =>
            options.DefaultEntryOptions = new HybridCacheEntryOptions()
            {
                Expiration = hybridCacheSettings!.Expiration,
                LocalCacheExpiration = hybridCacheSettings.LocalCacheExpiration,
                Flags = hybridCacheSettings.Flags,
            }
        );
    }

    private static void ConfigurePaymentGateway(IServiceCollection services)
    {
        services.AddMercadoPagoGateway();
        services.AddFakePaymentGateway();
        services.AddScoped<IPaymentGatewayFactoryMethod, PaymentGatewayFactory>();
    }
}
