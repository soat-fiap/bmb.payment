using System.Collections.ObjectModel;
using Bmb.Domain.Core.Entities;
using Bmb.Payment.Api.Model;
using Bmb.Payment.Controllers;
using Bmb.Payment.Controllers.Dto;
using PaymentType = Bmb.Domain.Core.ValueObjects.PaymentType;

namespace Bmb.Payment.Api;

internal static class Mappers
{
    /// <summary>
    /// Convert CreatePaymentRequest to CreateOrderPaymentRequestDto.
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    internal static CreateOrderPaymentRequestDto ToDomain(this CreatePaymentRequest request)
    {
        return new CreateOrderPaymentRequestDto(request.OrderId, (PaymentType)request.PaymentType);
    }

    // internal static OrderListItemDto ToOrderListViewModel(this Order order)
    // {
    //     return new OrderListItemDto(order.Id, order.TrackingCode.Value, order.Total,
    //         (OrderStatusDto)order.Status,
    //         order.Created,
    //         order.Updated);
    // }

    // internal static IReadOnlyCollection<OrderListItemDto> ToOrderListViewModel(this ReadOnlyCollection<Order> orders)
    // {
    //     return orders.Select(o => o.ToOrderListViewModel()).ToList();
    // }

    // internal static OrderDetailDto? ToOrderViewModel(this Order? order)
    // {
    //     if (order is null) return null;
    //     return new OrderDetailDto
    //     {
    //         Id = order.Id,
    //         TrackingCode = order.TrackingCode.Value,
    //         Total = order.Total,
    //         Status = (OrderStatusDto)order.Status,
    //         CreationDate = order.Created,
    //         LastUpdate = order.Updated,
    //         OrderItems = order.OrderItems.Select(o => o.ToOrderItemViewModel()).ToList(),
    //         Customer = order.Customer is null ? null : order.Customer!.FromEntityToDto()
    //     };
    // }

    // internal static OrderItemDto ToOrderItemViewModel(this OrderItem orderItem)
    // {
    //     return new OrderItemDto()
    //     {
    //         OrderId = orderItem.OrderId,
    //         ProductId = orderItem.ProductId,
    //         Quantity = orderItem.Quantity,
    //         UnitPrice = orderItem.UnitPrice,
    //         ProductName = orderItem.ProductName
    //     };
    // }
}
