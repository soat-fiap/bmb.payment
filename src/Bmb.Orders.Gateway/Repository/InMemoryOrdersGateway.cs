using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.Core;

namespace Bmb.Orders.Gateway.Repository;

[ExcludeFromCodeCoverage]
internal class InMemoryOrdersGateway : IOrdersGateway
{
    public Task<OrderDto?> GetCopyAsync(Guid orderId)
    {
        var order = new OrderDto(orderId, null, new List<OrderItemDto>(), OrderStatus.PaymentPending, "ABC-123", null,
            10);
        return Task.FromResult(order)!;
    }

    public Task SaveCopyAsync(OrderDto order)
    {
        return Task.CompletedTask;
    }
}