using Bmb.Domain.Core.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Bmb.Payment.Api;

/// <summary>
/// Filter class to handle domain exceptions.
/// </summary>
public class DomainExceptionFilter : IExceptionFilter
{
    private readonly ILogger<DomainExceptionFilter> _logger;

    /// <summary>
    /// DomainExceptionFilter
    /// </summary>
    /// <param name="logger">Logger</param>
    public DomainExceptionFilter(ILogger<DomainExceptionFilter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// On Exception handler
    /// </summary>
    /// <param name="context"></param>
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not DomainException) return;
        _logger.LogError(context.Exception, "An unhandled domain exception has occurred.");

        if (context.Exception is EntityNotFoundException)
        {
            context.Result = new NotFoundObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "The requested resource was not found.",
                Detail = context.Exception.Message,
            });
        }
        else
        {
            context.Result = new BadRequestObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "The request could not be processed.",
                Detail = context.Exception.Message,
            });
        }
    }
}
