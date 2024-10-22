using Bmb.Domain.Core.ValueObjects;

namespace Bmb.Payment.Core.Contracts;

public interface IPaymentGateway
{
    Task<Domain.Core.Entities.Payment> CreatePaymentAsync(OrderDto order);

    Task<PaymentStatus?> GetPaymentStatusAsync(string paymentId);
}