using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.Core.Contracts;

namespace Bmb.Payment.Application.UseCases;

public class UpdatePaymentStatusUseCase : IUpdatePaymentStatusUseCase
{
    private readonly IPaymentRepository _paymentRepository;

    public UpdatePaymentStatusUseCase(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<bool> Execute(Bmb.Domain.Core.Entities.Payment? payment, PaymentStatus status)
    {
        if (payment is not null && payment.Status
                is not PaymentStatus.Approved
                and not PaymentStatus.Cancelled
                and not PaymentStatus.Rejected)
        {
            payment.Status = status;
            payment.Updated = DateTime.UtcNow;

            var paymentStatusUpdated = await _paymentRepository.UpdatePaymentStatusAsync(payment);

            if (paymentStatusUpdated && payment.IsApproved())
            {
                // DomainEventTrigger.RaisePaymentConfirmed(payment);
                // TODO MassTransit 
                // await _updateOrderStatusUseCase.Execute(payment.OrderId, OrderStatus.Received);
            }

            return paymentStatusUpdated;
        }

        return false;
    }
}
