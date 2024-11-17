using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.Domain;

namespace Bmb.Orders.Gateway.Repository;

[ExcludeFromCodeCoverage]
internal class InMemoryOrdersGateway : IOrdersGateway
{
    public Task<OrderDto?> GetCopyAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = new OrderDto(orderId, null, new List<OrderItemDto>(), OrderStatus.PaymentPending, "ABC-123", null,
            10);
        return Task.FromResult(order)!;
    }

    public Task SaveCopyAsync(OrderDto order, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}