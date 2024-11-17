using Bmb.Domain.Core.Entities;

namespace Bmb.Payment.Domain;

public interface IOrdersGateway
{
    Task<OrderDto?> GetCopyAsync(Guid orderId, CancellationToken cancellationToken = default);
    
    Task SaveCopyAsync(OrderDto order, CancellationToken cancellationToken = default);
}