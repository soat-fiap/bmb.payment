using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.Interfaces;
using Bmb.Domain.Core.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Payment.FakePayment.Gateway;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddFakePaymentGateway(this IServiceCollection services)
    {
        services.AddKeyedScoped<IPaymentGateway, FakePaymentGatewayService>($"Payment-{nameof(PaymentType.Test)}");
    }
}
