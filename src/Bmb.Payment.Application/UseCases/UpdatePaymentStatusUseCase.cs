using Bmb.Domain.Core.Events;
using Bmb.Domain.Core.Events.Notifications;
using Bmb.Payment.Domain.Contracts;
using Bmb.Payment.Domain.ValueObjects;

namespace Bmb.Payment.Application.UseCases;

public class UpdatePaymentStatusUseCase : IUpdatePaymentStatusUseCase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IDispatcher _dispatcher;

    public UpdatePaymentStatusUseCase(IPaymentRepository paymentRepository, IDispatcher dispatcher)
    {
        _paymentRepository = paymentRepository;
        _dispatcher = dispatcher;
    }

    public async Task<bool> Execute(Domain.Entities.Payment? payment, PaymentStatus status)
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
                await _dispatcher.PublishAsync(new OrderPaymentConfirmed(payment.Id,
                    payment.OrderId));
            }

            return paymentStatusUpdated;
        }

        return false;
    }
}