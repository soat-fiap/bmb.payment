using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.Controllers.Dto;

namespace Bmb.Payment.Controllers.Contracts;

public interface IPaymentService
{
    Task<PaymentDto> CreateOrderPaymentAsync(CreateOrderPaymentRequestDto command);

    Task<PaymentDto?> GetPaymentAsync(Guid paymentId);

    Task<PaymentDto?> GetPaymentAsync(string paymentId, PaymentType paymentType);

    Task<bool> SyncPaymentStatusWithGatewayAsync(string externalReference, PaymentType paymentType);
}


