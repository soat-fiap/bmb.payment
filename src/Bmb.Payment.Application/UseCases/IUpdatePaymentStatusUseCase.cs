using Bmb.Payment.Domain.ValueObjects;

namespace Bmb.Payment.Application.UseCases;

public interface IUpdatePaymentStatusUseCase
{
    Task<bool> Execute(Domain.Entities.Payment? payment, PaymentStatus status);
}
