using Microsoft.AspNetCore.Mvc.Filters;

namespace Bmb.Payment.MercadoPago.Gateway.Security;

public interface IMercadoPagoHmacSignatureValidator
{
    bool TryToValidate(AuthorizationFilterContext context, out string reasonToFail);
}