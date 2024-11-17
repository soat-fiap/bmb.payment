using Bmb.Domain.Core.ValueObjects;
using PaymentStatus = Bmb.Payment.Domain.ValueObjects.PaymentStatus;
using ValueObjects_PaymentStatus = Bmb.Payment.Domain.ValueObjects.PaymentStatus;

namespace Bmb.Payment.Domain.Contracts;

public interface IPaymentGateway
{
    Task<Entities.Payment> CreatePaymentAsync(OrderDto order);

    Task<ValueObjects_PaymentStatus?> GetPaymentStatusAsync(string paymentId);
}