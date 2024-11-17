namespace Bmb.Payment.Domain.ValueObjects;

public enum PaymentStatus
{
    Pending = 0,
    InProgress = 1,
    Approved = 2,
    Rejected = 3,
    Paid = 4,
    Cancelled = 5
}
