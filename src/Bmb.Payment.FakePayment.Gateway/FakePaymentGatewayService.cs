using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.Domain;
using Bmb.Payment.Domain.Contracts;
using Bmb.Payment.Domain.ValueObjects;

namespace Bmb.Payment.FakePayment.Gateway;

[ExcludeFromCodeCoverage]
public class FakePaymentGatewayService : IPaymentGateway
{
    public Task<Domain.Entities.Payment> CreatePaymentAsync(OrderDto order)
    {
        var id = Guid.NewGuid();
        return Task.FromResult(new Domain.Entities.Payment
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
