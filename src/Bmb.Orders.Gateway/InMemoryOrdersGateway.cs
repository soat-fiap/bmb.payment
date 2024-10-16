using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.Entities;
using Bmb.Domain.Core.ValueObjects;

namespace Bmb.Orders.Gateway;

[ExcludeFromCodeCoverage]
public class InMemoryOrdersGateway: IOrdersGateway
{
    public Task<Order?> GetAsync(Guid orderId)
    {
        var order = new Order(orderId, null, OrderStatus.PaymentPending, "ABC-123", DateTime.UtcNow, DateTime.UtcNow);
        return Task.FromResult(order)!;
    }
}