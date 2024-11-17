using Bmb.Domain.Core.Base;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.Domain.ValueObjects;
using PaymentStatus = Bmb.Payment.Domain.ValueObjects.PaymentStatus;
using PaymentType = Bmb.Payment.Domain.ValueObjects.PaymentType;
using ValueObjects_PaymentStatus = Bmb.Payment.Domain.ValueObjects.PaymentStatus;
using ValueObjects_PaymentType = Bmb.Payment.Domain.ValueObjects.PaymentType;

namespace Bmb.Payment.Domain.Entities;

public class Payment : Entity<PaymentId>, IAggregateRoot
{
    public ValueObjects_PaymentType PaymentType { get; set; } = ValueObjects_PaymentType.Test;

    public Guid OrderId { get; set; }

    public string ExternalReference { get; set; }

    public string QrCode { get; set; }

    public decimal Amount { get; set; }

    public ValueObjects_PaymentStatus Status { get; set; }

    public Payment()
    {
        Created = DateTime.UtcNow;
    }

    public Payment(PaymentId id, string qrCode, decimal amount, ValueObjects_PaymentType paymentType = ValueObjects_PaymentType.Test)
        : base(id)
    {
        Id = id;
        Status = ValueObjects_PaymentStatus.Pending;
        QrCode = qrCode;
        Amount = amount;
        Created = DateTime.UtcNow;
        PaymentType = paymentType;
    }

    public bool IsApproved() => Status == PaymentStatus.Approved;
}
