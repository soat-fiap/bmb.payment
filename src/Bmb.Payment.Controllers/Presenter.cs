using Bmb.Payment.Controllers.Dto;

namespace Bmb.Payment.Controllers;

public static class Presenter
{
    public static PaymentDto FromEntityToDto(this Domain.Entities.Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id.Value,
            OrderId = payment.OrderId,
            Status = (PaymentStatusDto)payment.Status,
            PaymentType = (PaymentTypeDto)payment.PaymentType,
            Amount = payment.Amount,
            ExternalReference = payment.ExternalReference,
            QrCode = payment.QrCode
        };
    }
}
