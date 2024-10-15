using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.Interfaces;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.MercadoPago.Gateway.Configuration;
using Bmb.Payment.MercadoPago.Gateway.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Payment.MercadoPago.Gateway;

[ExcludeFromCodeCoverage]
public static class ServiceCollectionsExtensions
{
    public static void AddMercadoPagoGateway(this IServiceCollection services)
    {
        services.AddSingleton<MercadoPagoOptions>(provider =>
        {
            var configuration = provider.GetService<IConfiguration>();
            var options = new MercadoPagoOptions();
            configuration.GetSection(MercadoPagoOptions.MercadoPago)
                .Bind(options);

            return options;
        });

        services.AddSingleton<IMercadoPagoHmacSignatureValidator, MercadoPagoHmacSignatureValidator>()
            .AddKeyedScoped<IPaymentGateway, MercadoPagoService>($"Payment-{nameof(PaymentType.MercadoPago)}");
    }
}
