using Bmb.Domain.Core.Entities;
using Bmb.Domain.Core.ValueObjects;

namespace Bmb.Orders.Gateway;

public class OrdersGateway : IOrdersGateway
{
    public Task<Order?> GetAsync(Guid orderId)
    {
        var order = new Order(orderId, null, OrderStatus.PaymentPending, "ABC-123", DateTime.UtcNow, DateTime.UtcNow);
        return Task.FromResult(order)!;
    }
}