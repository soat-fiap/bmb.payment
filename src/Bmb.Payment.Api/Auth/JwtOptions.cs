namespace Bmb.Payment.Api.Auth;

/// <summary>
/// JWT token config
/// </summary>
/// <param name="Issuer">Issuer</param>
/// <param name="Audience">Audience</param>
/// <param name="SigningKey">Signing key secret</param>
/// <param name="ExpirationSeconds">Expiration in seconds</param>
/// <param name="UseAccessToken">Flag to use AccessCode header instead of normal Authorization header.</param>
public record JwtOptions(
    string Issuer,
    string Audience,
    string SigningKey,
    int ExpirationSeconds,
    bool UseAccessToken
);
