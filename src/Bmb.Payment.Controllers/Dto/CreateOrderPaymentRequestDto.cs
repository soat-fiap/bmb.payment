using Bmb.Domain.Core.ValueObjects;

namespace Bmb.Payment.Controllers.Dto;

public record CreateOrderPaymentRequestDto(Guid OrderId, PaymentType PaymentType);       