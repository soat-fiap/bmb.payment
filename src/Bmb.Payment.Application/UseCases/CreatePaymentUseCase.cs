using Bmb.Domain.Core.Base;
using Bmb.Domain.Core.Interfaces;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Orders.Gateway;

namespace Bmb.Payment.Application.UseCases;

public class CreatePaymentUseCase : ICreatePaymentUseCase
{
    private readonly IPaymentGatewayFactoryMethod _paymentGatewayFactory;
    private readonly IOrdersGateway  _ordersGateway;

    public CreatePaymentUseCase(IPaymentGatewayFactoryMethod paymentGatewayFactory, IOrdersGateway ordersGateway)
    {
        _paymentGatewayFactory = paymentGatewayFactory;
        _ordersGateway = ordersGateway;
    }

    public async Task<Bmb.Domain.Core.Entities.Payment?> Execute(Guid orderId, PaymentType paymentType)
    {
        var order = await _ordersGateway.GetAsync(orderId);

        if (order is null)
            throw new EntityNotFoundException("Order not found.");

        if (order.PaymentId is not null)
            throw new DomainException("There's already a Payment for the order.");

        var paymentGateway = _paymentGatewayFactory.Create(paymentType);
        var payment = await paymentGateway.CreatePaymentAsync(order);
        if (payment != null)
        {
            // TODO MassTransit
            // DomainEventTrigger.RaisePaymentCreated(new PaymentCreated(payment));
        }
        return payment;
    }
}
