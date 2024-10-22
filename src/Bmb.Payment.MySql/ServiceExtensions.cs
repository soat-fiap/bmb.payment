using System.Diagnostics.CodeAnalysis;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Bmb.Payment.Core.Contracts;
using Bmb.Payment.MySql.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Payment.MySql;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static void ConfigurePersistenceApp(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IAmazonDynamoDB>(_ =>
        {
            var dynamoDbSettings = configuration.GetSection("AwsSettings").Get<AwsSettings>();
            return new AmazonDynamoDBClient(
                new BasicAWSCredentials(dynamoDbSettings!.ClientId, dynamoDbSettings.ClientSecret),
                RegionEndpoint.GetBySystemName(dynamoDbSettings.Region));
        });
    }
}