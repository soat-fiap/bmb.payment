using Bmb.Payment.Controllers.Dto;
using Microsoft.Build.Framework;

namespace Bmb.Payment.Api.Model;

/// <summary>
/// Create payment request.
/// </summary>
public class CreatePaymentRequest
{
    /// <summary>
    /// Order id.
    /// </summary>
    [Required]
    public Guid OrderId { get; set; }

    /// <summary>
    /// Payment type
    /// </summary>
    [Required]
    public PaymentTypeDto PaymentType { get; set; }
}
