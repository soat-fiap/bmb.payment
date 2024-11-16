using Bmb.Domain.Core.Events.Integration;
using Bmb.Payment.Core;

namespace Bmb.Payment.Masstransit;

internal static class Presenter
{
    internal static OrderDto ToDto(this OrderCreated order)
    {
        return new OrderDto(order.Id, order.Customer?.ToDto(), order.Items.ToDto(), order.Status,
            order.OrderTrackingCode, order.PaymentId, order.Total);
    }

    private static CustomerDto? ToDto(this OrderCreated.CustomerReplicaDto? customer)
    {
        return customer is null ? null : new CustomerDto(customer.Id, customer.Cpf, customer.Name, customer.Email);
    }

    private static List<OrderItemDto> ToDto(this List<OrderCreated.OrderItemReplicaDto>? items)
    {
        var dtoItems = new List<OrderItemDto>();
        if (items != null)
            dtoItems.AddRange(items.Select(x =>
                new OrderItemDto(x.Id, x.OrderId, x.ProductName, x.UnitPrice, x.Quantity)));
        return dtoItems;
    }
}