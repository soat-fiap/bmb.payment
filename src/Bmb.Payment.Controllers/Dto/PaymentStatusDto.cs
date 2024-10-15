namespace Bmb.Payment.Controllers.Dto;

/// <summary>
/// Represents the payment status view model.
/// </summary>
public enum PaymentStatusDto
{
    Pending = 0,
    InProgress = 1,
    Approved = 2,
    Rejected = 3,
    Paid = 4,
    Cancelled = 5
}

