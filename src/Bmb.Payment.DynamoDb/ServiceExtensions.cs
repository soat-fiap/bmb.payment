using System.Diagnostics.CodeAnalysis;
using Bmb.Payment.Core.Contracts;
using Bmb.Payment.DynamoDb.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Payment.DynamoDb;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static void ConfigurePersistenceApp(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IPaymentRepository, PaymentRepository>();
    }
}