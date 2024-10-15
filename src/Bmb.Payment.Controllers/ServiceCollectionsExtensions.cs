using System.Diagnostics.CodeAnalysis;
using Bmb.Payment.Controllers.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Payment.Controllers;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddControllers(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService, PaymentService>();
    }
}
