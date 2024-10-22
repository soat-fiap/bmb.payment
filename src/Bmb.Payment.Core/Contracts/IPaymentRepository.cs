using Bmb.Domain.Core.ValueObjects;

namespace Bmb.Payment.Core.Contracts;

public interface IPaymentRepository
{
    Task<Domain.Core.Entities.Payment> SaveAsync(Domain.Core.Entities.Payment payment);

    Task<Domain.Core.Entities.Payment?> GetPaymentAsync(PaymentId paymentId);

    Task<Domain.Core.Entities.Payment?> GetPaymentAsync(string externalReference, PaymentType paymentType);

    Task<bool> UpdatePaymentStatusAsync(Domain.Core.Entities.Payment payment);
}
