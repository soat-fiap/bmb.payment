using Bmb.Domain.Core.ValueObjects;

namespace Bmb.Payment.Application.UseCases.Payment;

public interface IUpdatePaymentStatusUseCase
{
    Task<bool> Execute(Bmb.Domain.Core.Entities.Payment? payment, PaymentStatus status);
}
