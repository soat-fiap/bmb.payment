using Bmb.Domain.Core.ValueObjects;

namespace Bmb.Payment.Core.Contracts;

public interface IPaymentGatewayFactoryMethod
{
    IPaymentGateway Create(PaymentType paymentType);
}