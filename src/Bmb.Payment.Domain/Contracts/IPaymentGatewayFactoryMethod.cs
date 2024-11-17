using Bmb.Domain.Core.ValueObjects;
using PaymentType = Bmb.Payment.Domain.ValueObjects.PaymentType;
using ValueObjects_PaymentType = Bmb.Payment.Domain.ValueObjects.PaymentType;

namespace Bmb.Payment.Domain.Contracts;

public interface IPaymentGatewayFactoryMethod
{
    IPaymentGateway Create(ValueObjects_PaymentType paymentType);
}