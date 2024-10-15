using Bmb.Domain.Core.ValueObjects;

namespace Bmb.Payment.Application.UseCases.Payment;

public interface ICreatePaymentUseCase
{
    Task<Bmb.Domain.Core.Entities.Payment?> Execute(Guid orderId, PaymentType paymentType);
}
