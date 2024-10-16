using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Bmb.Payment.Api.Auth;

/// <summary>
/// Jwt token extensions methods
/// </summary>
[ExcludeFromCodeCoverage]
public static class JwtExtensions
{
    
    /// <summary>
    /// Configure Jtw token validation
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration</param>
    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration
            .GetSection("JwtOptions")
            .Get<JwtOptions>();

        services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                if (jwtOptions.UseAccessToken)
                {
                    options.Events = AccessTokenAuthEventsHandler.Instance;
                }

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    LogValidationExceptions = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey))
                };
            });
    }
}
