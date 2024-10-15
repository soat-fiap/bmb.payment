using System.Security.Cryptography;
using System.Text;
using Bmb.Payment.MercadoPago.Gateway.Configuration;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Bmb.Payment.MercadoPago.Gateway.Security;

public class MercadoPagoHmacSignatureValidator : IMercadoPagoHmacSignatureValidator
{
    private readonly MercadoPagoOptions _mercadoPagoOptions;
    private const string XSignatureHeaderName = "x-signature";
    private const string XRequestIdHeaderName = "x-request-id";

    public MercadoPagoHmacSignatureValidator(MercadoPagoOptions mercadoPagoOptions)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(mercadoPagoOptions.WebhookSecret);
        _mercadoPagoOptions = mercadoPagoOptions;
    }

    public bool TryToValidate(AuthorizationFilterContext context, out string reasonToFail)
    {
        reasonToFail = string.Empty;

        if (!TryValidateRequestParams(context, out var dataId, out var signature, out var xRequestIdHeader,
                out var reasonParamFail))
        {
            reasonToFail = reasonParamFail;
            return false;
        }

        if (!TryValidateSignature(signature, dataId, xRequestIdHeader, out var reasonSignatureFail))
        {
            reasonToFail = reasonSignatureFail;
            return false;
        }

        return true;
    }

    private bool TryValidateSignature(StringValues signature, StringValues dataId,
        StringValues xRequestIdHeader, out string reasonToFail)
    {
        try
        {
            reasonToFail = string.Empty;
            var tsHash = signature.ToString().Split(",");
            if (tsHash.Length != 2)
            {
                reasonToFail = "Webhook message not authorized. Signature format not valid";
                return false;
            }

            var ts = tsHash[0].Substring(3, tsHash[0].Length - 3);
            var hash = tsHash[1].Substring(3, tsHash[1].Length - 3);
            var manifest = $"id:{dataId};request-id:{xRequestIdHeader};ts:{ts};";

            var computedSignature = ComputeSignature(manifest, _mercadoPagoOptions.WebhookSecret);
            if (!string.Equals(hash, computedSignature, StringComparison.OrdinalIgnoreCase))
            {
                reasonToFail = "Webhook message not authorized. Signature didn't match.";
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            reasonToFail = e.StackTrace ?? "Unable to describe the error. Please check input parameters.";
            return false;
        }
    }

    private static bool TryValidateRequestParams(AuthorizationFilterContext context, out StringValues dataId,
        out StringValues signature, out StringValues xRequestIdHeader, out string reasonToFail)
    {
        signature = default;
        xRequestIdHeader = default;
        reasonToFail = string.Empty;

        if (!context.HttpContext.Request.Query.TryGetValue("data.id", out dataId) &&
            !context.HttpContext.Request.Query.TryGetValue("data_id", out dataId)
            || string.IsNullOrWhiteSpace(dataId))
        {
            reasonToFail = "Webhook message not authorized. Missing DataId";
            return false;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(XSignatureHeaderName, out signature) ||
            string.IsNullOrWhiteSpace(signature))
        {
            reasonToFail = "Webhook message not authorized. Missing signature";
            return false;
        }

        if (!context.HttpContext.Request.Headers.TryGetValue(XRequestIdHeaderName, out xRequestIdHeader) ||
            string.IsNullOrWhiteSpace(xRequestIdHeader))
        {
            reasonToFail = "Webhook message not authorized. Missing RequestId";
            return false;
        }

        return true;
    }

    private static string ComputeSignature(string data, string secret)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
        var computedHmac = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(computedHmac);
    }
}
