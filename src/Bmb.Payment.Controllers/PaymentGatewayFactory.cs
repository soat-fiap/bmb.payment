using Bmb.Domain.Core.Base;
using Bmb.Domain.Core.Interfaces;
using Bmb.Domain.Core.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace Bmb.Payment.Controllers;

public sealed class PaymentGatewayFactory(IServiceProvider provider) : IPaymentGatewayFactoryMethod
{
    public IPaymentGateway Create(PaymentType paymentType)
    {
        var gateway = paymentType switch
        {
            PaymentType.MercadoPago => provider.GetKeyedService<IPaymentGateway>(
                $"Payment-{nameof(PaymentType.MercadoPago)}"),
            PaymentType.Test => provider.GetKeyedService<IPaymentGateway>($"Payment-{nameof(PaymentType.Test)}"),
            _ => null
        };

        return gateway ?? throw new DomainException($"Payment Gateway Payment-{paymentType} not found.");
    }
}
