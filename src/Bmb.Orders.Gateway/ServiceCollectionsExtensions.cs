using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Bmb.Orders.Gateway;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddOrdersGateway(this IServiceCollection services)
    {
        // services.AddScoped<IOrdersGateway, InMemoryOrdersGateway>();
        services.AddRefitClient<IOrdersGateway>()
            .ConfigureHttpClient(c=>c.BaseAddress = new Uri("orders-api"));
    }
}
