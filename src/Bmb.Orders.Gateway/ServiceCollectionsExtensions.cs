using System.Diagnostics.CodeAnalysis;
using Bmb.Orders.Gateway.Repository;
using Bmb.Payment.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Orders.Gateway;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddOrdersGateway(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IOrdersGateway, OrderReplicaRepository>();
    }
}