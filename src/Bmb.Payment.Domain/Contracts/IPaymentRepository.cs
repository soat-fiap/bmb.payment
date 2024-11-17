using Bmb.Domain.Core.ValueObjects;
using PaymentType = Bmb.Payment.Domain.ValueObjects.PaymentType;
using ValueObjects_PaymentType = Bmb.Payment.Domain.ValueObjects.PaymentType;

namespace Bmb.Payment.Domain.Contracts;

public interface IPaymentRepository
{
    Task<Entities.Payment> SaveAsync(Entities.Payment payment);

    Task<Entities.Payment?> GetPaymentAsync(PaymentId paymentId);

    Task<Entities.Payment?> GetPaymentAsync(string externalReference, ValueObjects_PaymentType paymentType);

    Task<bool> UpdatePaymentStatusAsync(Entities.Payment payment);
}
