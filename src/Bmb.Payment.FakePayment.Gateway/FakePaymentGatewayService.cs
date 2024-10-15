using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.Entities;
using Bmb.Domain.Core.Interfaces;
using Bmb.Domain.Core.ValueObjects;

namespace Bmb.Payment.FakePayment.Gateway;

[ExcludeFromCodeCoverage]
public class FakePaymentGatewayService : IPaymentGateway
{
    public Task<Domain.Core.Entities.Payment> CreatePaymentAsync(Order order)
    {
        var id = Guid.NewGuid();
        return Task.FromResult(new Domain.Core.Entities.Payment
        {
            Id = new PaymentId(id),
            OrderId = order.Id,
            PaymentType = PaymentType.Test,
            Amount = order.Total,
            Created = DateTime.Now,
            Status = PaymentStatus.Pending,
            QrCode = "https://fake.qrcode.com",
            ExternalReference = id.ToString()
        });
    }

    public Task<PaymentStatus?> GetPaymentStatusAsync(string paymentId)
    {
        PaymentStatus? status = PaymentStatus.Approved;
        return Task.FromResult(status);
    }
}
