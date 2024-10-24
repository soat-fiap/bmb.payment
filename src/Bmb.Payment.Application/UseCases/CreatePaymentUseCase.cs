using Bmb.Domain.Core.Base;
using Bmb.Domain.Core.Events.Notifications;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.Core;
using Bmb.Payment.Core.Contracts;

namespace Bmb.Payment.Application.UseCases;

public class CreatePaymentUseCase : ICreatePaymentUseCase
{
    private readonly IPaymentGatewayFactoryMethod _paymentGatewayFactory;
    private readonly IOrdersGateway  _ordersGateway;
    private readonly IDispatcher _dispatcher;

    public CreatePaymentUseCase(IPaymentGatewayFactoryMethod paymentGatewayFactory,
        IOrdersGateway ordersGateway,
        IDispatcher dispatcher)
    {
        _paymentGatewayFactory = paymentGatewayFactory;
        _ordersGateway = ordersGateway;
        _dispatcher = dispatcher;
    }

    public async Task<Bmb.Domain.Core.Entities.Payment?> Execute(Guid orderId, PaymentType paymentType)
    {
        var order = await _ordersGateway.GetCopyAsync(orderId);

        if (order is null)
            throw new EntityNotFoundException("Order not found.");

        if (order.PaymentId is not null)
            throw new DomainException("There's already a Payment for the order.");

        var paymentGateway = _paymentGatewayFactory.Create(paymentType);
        var payment = await paymentGateway.CreatePaymentAsync(order);
        if (payment != null)
        {
            await _dispatcher.PublishAsync(new PaymentCreated(payment.Id.Value, payment.OrderId));
        }
        return payment;
    }
}
