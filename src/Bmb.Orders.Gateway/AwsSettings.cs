using System.Diagnostics.CodeAnalysis;

namespace Bmb.Orders.Gateway;

/// <summary>
/// DynamoDb settings
/// </summary>
[ExcludeFromCodeCoverage]
internal class AwsSettings
{
    /// <summary>
    /// AWS Region
    /// </summary>
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