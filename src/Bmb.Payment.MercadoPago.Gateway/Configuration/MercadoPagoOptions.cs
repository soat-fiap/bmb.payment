using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Bmb.Payment.MercadoPago.Gateway.Configuration;

[ExcludeFromCodeCoverage]
public class MercadoPagoOptions
{
    public const string MercadoPago = "MercadoPago";

    [Required]
    [MinLength(1)]
    public string WebhookSecret { get; set; }

    [Required]
    [MinLength(1)]
    public string AccessToken { get; set; }

    [MinLength(1)]
    public string? NotificationUrl { get; set; }
}
