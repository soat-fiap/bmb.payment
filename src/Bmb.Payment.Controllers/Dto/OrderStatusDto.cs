namespace Bmb.Payment.Controllers.Dto;

/// <summary>
/// Orders statuses
/// </summary>
public enum OrderStatusDto
{
    PaymentPending = 0,
    Received = 1,
    InPreparation = 2,
    Ready = 3,
    Completed = 4
}
