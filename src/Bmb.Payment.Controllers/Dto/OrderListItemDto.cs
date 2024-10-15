namespace Bmb.Payment.Controllers.Dto;

public record OrderListItemDto(
    Guid Id,
    string TrackingCode,
    decimal Total,
    OrderStatusDto Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

