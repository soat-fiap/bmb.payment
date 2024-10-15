using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Orders.Gateway;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddOrdersGateway(this IServiceCollection services)
    {
        services.AddScoped<IOrdersGateway, OrdersGateway>();
    }
}
