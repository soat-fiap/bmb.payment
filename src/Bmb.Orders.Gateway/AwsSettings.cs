using System.Diagnostics.CodeAnalysis;

namespace Bmb.Orders.Gateway;

/// <summary>
/// DynamoDb settings
/// </summary>
[ExcludeFromCodeCoverage]
public record AwsSettings(string Region, string ClientSecret, string ClientId);