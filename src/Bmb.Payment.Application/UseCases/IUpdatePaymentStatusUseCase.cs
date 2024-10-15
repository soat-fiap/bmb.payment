using Bmb.Domain.Core.ValueObjects;

namespace Bmb.Payment.Application.UseCases;

public interface IUpdatePaymentStatusUseCase
{
    Task<bool> Execute(Bmb.Domain.Core.Entities.Payment? payment, PaymentStatus status);
}
