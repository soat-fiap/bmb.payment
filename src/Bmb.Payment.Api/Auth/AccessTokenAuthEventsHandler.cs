using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Bmb.Payment.Api.Auth;

/// <summary>
/// Singleton class handler of events related to JWT authentication
/// </summary>
internal class AccessTokenAuthEventsHandler : JwtBearerEvents
{
    private const string BearerPrefix = "Bearer ";

    private AccessTokenAuthEventsHandler() => OnMessageReceived = MessageReceivedHandler;

    /// <summary>
    /// Gets single available instance of <see cref="AccessTokenAuthEventsHandler"/>
    /// </summary>
    public static AccessTokenAuthEventsHandler Instance { get; } = new ();

    internal Task MessageReceivedHandler(MessageReceivedContext context)
    {
        if (context.Request.Headers.TryGetValue("AccessToken", out var headerValue) &&
            !string.IsNullOrWhiteSpace(headerValue))
        {
            var accessToken = headerValue.ToString();
            context.Token = accessToken.StartsWith(BearerPrefix) ? accessToken[BearerPrefix.Length..] : accessToken;
        }

        return Task.CompletedTask;
    }
}
