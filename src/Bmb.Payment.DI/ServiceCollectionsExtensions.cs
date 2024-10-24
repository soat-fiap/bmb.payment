using System.Diagnostics.CodeAnalysis;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using Bmb.Orders.Gateway;
using Bmb.Payment.Application;
using Bmb.Payment.Controllers;
using Bmb.Payment.Core.Contracts;
using Bmb.Payment.DI.HealthChecks;
using Bmb.Payment.FakePayment.Gateway;
using Bmb.Payment.MercadoPago.Gateway;
using Bmb.Payment.MySql;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using AwsSettings = Bmb.Orders.Gateway.AwsSettings;

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
        serviceCollection.AddOrdersGateway(configuration);
        serviceCollection.AddDynamoDbConnection(configuration);
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


    public static void ConfigureHealthCheck(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddDynamoDbHealthCheck("Payments")
            .AddDynamoDbHealthCheck("Payment-OrdersReplica");
    }

    private static void AddDynamoDbConnection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAmazonDynamoDB>(_ =>
        {
            var dynamoDbSettings = configuration.GetSection("AwsSettings").Get<AwsSettings>();
            return new AmazonDynamoDBClient(
                new BasicAWSCredentials(dynamoDbSettings!.ClientId, dynamoDbSettings.ClientSecret),
                RegionEndpoint.GetBySystemName(dynamoDbSettings.Region));
        });
    }
}   