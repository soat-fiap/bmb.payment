using Bmb.Payment.Domain.ValueObjects;

namespace Bmb.Payment.Controllers.Dto;

public record CreateOrderPaymentRequestDto(Guid OrderId, PaymentType PaymentType);       