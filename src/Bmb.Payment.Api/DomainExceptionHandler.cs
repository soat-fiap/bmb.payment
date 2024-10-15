using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.Base;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Bmb.Payment.Api;

/// <summary>
/// DomainExceptionHandler
/// </summary>
/// <param name="logger">Logger</param>
[ExcludeFromCodeCoverage]
public class DomainExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    /// <summary>
    /// Handler domain exceptions
    /// </summary>
    /// <param name="httpContext"></param>
    /// <param name="exception"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogWarning(exception, "Exception occurred: {Message}", exception.Message);

        if (exception is not DomainException)
            return false;

        var problemDetails = new ProblemDetails
        {
            Status = exception is EntityNotFoundException ? StatusCodes.Status404NotFound : StatusCodes.Status400BadRequest,
            Title = "The request could not be completed.",
            Detail = exception.Message,
        };
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
