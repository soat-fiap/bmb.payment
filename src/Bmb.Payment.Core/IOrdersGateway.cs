using Bmb.Domain.Core.Entities;

namespace Bmb.Payment.Core;

public interface IOrdersGateway
{
    Task<OrderDto?> GetCopyAsync(Guid orderId);
    
    Task SaveCopyAsync(OrderDto order);
}