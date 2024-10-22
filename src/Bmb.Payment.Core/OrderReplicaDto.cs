using Bmb.Domain.Core.ValueObjects;

namespace Bmb.Payment.Core;

public record OrderReplicaDto(Guid OrderId, OrderDto OrderDetails);

public class OrderDto
{
    public Guid Id { get; set; }
    public CustomerDto? Customer { get; set; }
    public List<OrderItemDto> Items { get; set; } = null!;
    public OrderStatus Status { get; set; }
    public string OrderTrackingCode { get; set; } = null!;
    public PaymentId PaymentId { get; set; } = null!;
    public decimal Total { get; set; }

    public OrderDto()
    {
    }

    public OrderDto(Guid id, CustomerDto? customer, List<OrderItemDto> items, OrderStatus status,
        string orderTrackingCode, PaymentId paymentId, decimal total)
    {
        Id = id;
        Customer = customer;
        Items = items;
        Status = status;
        OrderTrackingCode = orderTrackingCode;
        PaymentId = paymentId;
        Total = total;
    }
}

public record CustomerDto(Guid Id, string Cpf, string Name, string Email);

public record OrderItemDto(Guid Id, Guid OrderId, string ProductName, decimal UnitPrice, int Quantity);