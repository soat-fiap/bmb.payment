using Bmb.Payment.MercadoPago.Gateway.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Bmb.Payment.Api.Auth;

/// <summary>
/// Validate Mercado Pago Webhook message
/// </summary>
public class MercadoPagoMessageAuthorizationFilter : IAuthorizationFilter
{
    private readonly ILogger<MercadoPagoMessageAuthorizationFilter> _logger;
    private readonly IMercadoPagoHmacSignatureValidator _mercadoPagoHmacSignatureValidator;

    /// <summary>
    /// Initialize class
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mercadoPagoHmacSignatureValidator"></param>
    public MercadoPagoMessageAuthorizationFilter(ILogger<MercadoPagoMessageAuthorizationFilter> logger,
        IMercadoPagoHmacSignatureValidator mercadoPagoHmacSignatureValidator)
    {
        _logger = logger;
        _mercadoPagoHmacSignatureValidator = mercadoPagoHmacSignatureValidator;
    }

    /// <summary>
    /// Validate Mercado Pago Webhook message
    /// </summary>
    /// <param name="context"></param>
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        try
        {
            if (_mercadoPagoHmacSignatureValidator.TryToValidate(context, out var reasonToFail))
            {
                return;
            }

            context.Result = new UnauthorizedResult();
            _logger.LogWarning("Webhook request not authorized: {@ReasonToFail} {@Query}, {Body}", reasonToFail, context.HttpContext.Request.Query, context.HttpContext.Request.Body);
        }
        catch (Exception e)
        {
            _logger.LogError(e,"Webhook message not authorized. {@Query}, {Body}", context.HttpContext.Request.Query, context.HttpContext.Request.Body);
            context.Result = new UnauthorizedResult();
        }
    }
}
