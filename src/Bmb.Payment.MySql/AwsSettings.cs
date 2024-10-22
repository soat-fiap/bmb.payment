using System.Diagnostics.CodeAnalysis;

namespace Bmb.Payment.MySql;

/// <summary>
/// DynamoDb settings
/// </summary>
[ExcludeFromCodeCoverage]
public class AwsSettings
{
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// AWS Secret Id
    /// </summary>
    public string ClientSecret { get; set; } = string.Empty;

    /// <summary>
    /// AWS Client Id
    /// </summary>
    public string ClientId { get; set; } = string.Empty;
}