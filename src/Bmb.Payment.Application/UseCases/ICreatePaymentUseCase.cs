namespace Bmb.Payment.Application.UseCases;

public interface ICreatePaymentUseCase
{
    Task<Domain.Entities.Payment?> Execute(Guid orderId, Domain.ValueObjects.PaymentType paymentType);
}
