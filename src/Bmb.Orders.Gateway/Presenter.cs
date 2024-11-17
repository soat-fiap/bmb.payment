using System.Text.Json;
using Amazon.DynamoDBv2.Model;
using Bmb.Domain.Core.Entities;
using Bmb.Payment.Domain;

namespace Bmb.Orders.Gateway;

public static class Presenter
{
    internal static OrderDto? ToDomain(this Dictionary<string, AttributeValue> item)
    {
        return JsonSerializer.Deserialize<OrderDto>(item["OrderDetails"].S);
    }

    internal static Dictionary<string, AttributeValue> ToDynamoDb(this OrderReplicaDto order)
    {
        return new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue(order.OrderId.ToString()) },
            { "OrderDetails", new AttributeValue(JsonSerializer.Serialize(order.OrderDetails)) },
            { "ExpireAt", new AttributeValue(DateTimeOffset.UtcNow.AddHours(6).ToUnixTimeSeconds().ToString()) }
        };
    }

    public static OrderReplicaDto ToReplicaDto(this Order order)
    {
        var orderDto = order.ToDto();
        return new OrderReplicaDto(order.Id, orderDto);
    }
    
    public static OrderDto ToDto(this Order order)
    {
        var customer = order.Customer is null
            ? null
            : new CustomerDto(order.Customer.Id, order.Customer.Cpf, order.Customer.Name!, order.Customer.Email!);
        var orderItems = order.OrderItems
            .Select(x => new OrderItemDto(x.Id, x.OrderId, x.ProductName, x.UnitPrice, x.Quantity)).ToList();
        return new OrderDto(order.Id, customer, orderItems, order.Status, order.TrackingCode.Value,
            order.PaymentId!, order.Total);
    }

    internal static Dictionary<string, AttributeValue> ToDynamoDb(this OrderDto order)
    {
        return new OrderReplicaDto(order.Id, order).ToDynamoDb();
    }
}