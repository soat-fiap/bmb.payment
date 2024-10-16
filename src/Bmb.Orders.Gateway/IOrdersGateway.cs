using Bmb.Domain.Core.Entities;
using Refit;

namespace Bmb.Orders.Gateway;

public interface IOrdersGateway
{
    [Get("/api/{orderId}")]
    Task<Order?> GetAsync(Guid orderId);
}