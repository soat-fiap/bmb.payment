using Bmb.Domain.Core.Base;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bmb.Payment.Api.Test;

[TestSubject(typeof(DomainExceptionFilter))]
public class DomainExceptionFilterTest
{
    private readonly Mock<ILogger<DomainExceptionFilter>> _loggerMock;
    private readonly DomainExceptionFilter _filter;

    public DomainExceptionFilterTest()
    {
        _loggerMock = new Mock<ILogger<DomainExceptionFilter>>();
        _filter = new DomainExceptionFilter(_loggerMock.Object);
    }

    [Fact]
    public void OnException_ShouldReturnNotFound_WhenEntityNotFoundExceptionIsThrown()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var context = new ExceptionContext(new ActionContext
        {
            HttpContext = httpContext,
            RouteData = httpContext.GetRouteData(),
            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
        }, new List<IFilterMetadata>())
        {
            Exception = new EntityNotFoundException("Entity not found")
        };

        // Act
        _filter.OnException(context);

        // Assert
        context.Result.Should().BeOfType<NotFoundObjectResult>();
        var result = context.Result as NotFoundObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        result.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = result.Value as ProblemDetails;
        problemDetails!.Title.Should().Be("The requested resource was not found.");
        problemDetails.Detail.Should().Be("Entity not found");
    }

    [Fact]
    public void OnException_ShouldReturnBadRequest_WhenDomainExceptionIsThrown()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var context = new ExceptionContext(new ActionContext
        {
            HttpContext = httpContext,
            RouteData = httpContext.GetRouteData(),
            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
        }, new List<IFilterMetadata>())
        {
            Exception = new DomainException("Domain error")
        };

        // Act
        _filter.OnException(context);

        // Assert
        context.Result.Should().BeOfType<BadRequestObjectResult>();
        var result = context.Result as BadRequestObjectResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.Value.Should().BeOfType<ProblemDetails>();
        var problemDetails = result.Value as ProblemDetails;
        problemDetails!.Title.Should().Be("The request could not be processed.");
        problemDetails.Detail.Should().Be("Domain error");
    }

    [Fact]
    public void OnException_ShouldNotHandleException_WhenNotDomainException()
    {
        // Arrange
        var httpContext = new DefaultHttpContext();
        var context = new ExceptionContext(new ActionContext
        {
            HttpContext = httpContext,
            RouteData = httpContext.GetRouteData(),
            ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
        }, new List<IFilterMetadata>())
        {
            Exception = new Exception("General error")
        };

        // Act
        _filter.OnException(context);

        // Assert
        context.Result.Should().BeNull();
    }
}