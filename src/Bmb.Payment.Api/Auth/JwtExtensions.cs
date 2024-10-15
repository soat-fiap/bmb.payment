using System.Security.Claims;
using System.Text;
using Bmb.Payment.Controllers.Dto;
using Microsoft.IdentityModel.Tokens;

namespace Bmb.Payment.Api.Auth;

/// <summary>
/// Jwt token extensions methods
/// </summary>
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

    // https://stackoverflow.com/a/55740879/2921329
    /// <summary>
    /// Get customer details from Jwt Claims
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static CustomerDto? GetCustomerFromClaims(this HttpContext context)
    {
        if (Guid.TryParse(
                context.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value,
                out var customerId))
        {
            var email = context.User.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;
            var name = context.User.Claims.First(claim => claim.Type is ClaimTypes.Name or "name" ).Value;
            var cpf = context.User.Claims.First(claim => claim.Type == "cpf").Value;

            return new CustomerDto(customerId, cpf, name, email);
        }

        return default;
    }
}
