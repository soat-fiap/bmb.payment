using System.Diagnostics.CodeAnalysis;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Bmb.Orders.Gateway.Repository;
using Bmb.Payment.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Orders.Gateway;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddOrdersGateway(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAmazonDynamoDB>(_ =>
        {
            var dynamoDbSettings = configuration
                .GetSection("AwsSettings")
                .Get<AwsSettings>();
            
            return new AmazonDynamoDBClient(
                new BasicAWSCredentials(dynamoDbSettings!.ClientId, dynamoDbSettings.ClientSecret),
                RegionEndpoint.GetBySystemName(dynamoDbSettings.Region));
        });
        services.AddScoped<IOrdersGateway, InMemoryOrdersGateway>();
    }
}