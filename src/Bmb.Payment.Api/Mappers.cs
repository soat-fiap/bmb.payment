using Bmb.Payment.Api.Model;
using Bmb.Payment.Controllers.Dto;
using Bmb.Payment.Domain.ValueObjects;

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
}
