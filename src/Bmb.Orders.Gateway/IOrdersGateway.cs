using Bmb.Domain.Core.Entities;

namespace Bmb.Orders.Gateway;

public interface IOrdersGateway
{
    Task<Order?> GetAsync(Guid orderId);
}
