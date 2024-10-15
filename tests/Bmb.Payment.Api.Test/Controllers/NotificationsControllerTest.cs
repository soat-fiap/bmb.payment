using AutoFixture;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.Api.Controllers;
using Bmb.Payment.Controllers;
using Bmb.Payment.Controllers.Contracts;
using Bmb.Payment.Controllers.Dto;
using Bmb.Payment.MercadoPago.Gateway.Model;
using FluentAssertions;
using FluentAssertions.Execution;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bmb.Payment.Api.Test.Controllers;

[TestSubject(typeof(NotificationsController))]
public class NotificationsControllerTest
{
    private readonly NotificationsController _target;
    private readonly Mock<IPaymentService> _mockPaymentService;
    private readonly Mock<HttpResponse> _mockResponse;

    public NotificationsControllerTest()
    {
        var mockHttpContext = new Mock<HttpContext>();
        _mockResponse = new Mock<HttpResponse>();
        mockHttpContext.SetupGet(x => x.Response).Returns(_mockResponse.Object);

        _mockPaymentService = new Mock<IPaymentService>();
        _target = new NotificationsController(_mockPaymentService.Object, Mock.Of<ILogger<NotificationsController>>())
        {
            ControllerContext = new ControllerContext()
            {
                HttpContext = mockHttpContext.Object
            }
        };
    }

    [Fact]
    public async Task PostMercadoPago_PaymentEvent_Processed()
    {
        // Arrange
        var payment = new Fixture().Create<Domain.Core.Entities.Payment>();
        var @event = new Fixture().Build<MercadoPagoWebhookEvent>()
            .With(e => e.Action, "payment.updated")
            .With(e => e.Data, new MercadoPagoWebhookData
            {
                Id = payment.ExternalReference
            })
            .Create();

        _mockPaymentService.Setup(s => s.GetPaymentAsync(@event.Data.Id, PaymentType.MercadoPago))
            .ReturnsAsync(payment.FromEntityToDto())
            .Verifiable();

        _mockPaymentService.Setup(s => s.SyncPaymentStatusWithGatewayAsync(@event.Data.Id, PaymentType.MercadoPago))
            .ReturnsAsync(true)
            .Verifiable();

        // Act
        var response = await _target.Post(@event, string.Empty, string.Empty);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Should().BeOfType<OkResult>();
        }
    }

    [Fact]
    public async Task PostMercadoPago_PaymentEvent_PaymentNotFound()
    {
        // Arrange
        var @event = new Fixture().Create<MercadoPagoWebhookEvent>();
        _mockPaymentService.Setup(s => s.GetPaymentAsync(@event.Data.Id, PaymentType.MercadoPago))
            .ReturnsAsync((PaymentDto?)null)
            .Verifiable();

        // Act
        var response = await _target.Post(@event, string.Empty, string.Empty);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Should().BeOfType<OkResult>();
            _mockResponse.Verify(x => x.OnCompleted(It.IsAny<Func<object, Task>>(), It.IsAny<object>()), Times.Never);
        }
    }

    [Fact]
    public async Task FakePayment_PaymentEvent_PaymentNotFound()
    {
        // Arrange
        var paymentExternalReference = Guid.NewGuid().ToString();
        _mockPaymentService.Setup(s => s.GetPaymentAsync(paymentExternalReference, PaymentType.Test))
            .ReturnsAsync((PaymentDto?)null)
            .Verifiable();

        // Act
        var response = await _target.Post(paymentExternalReference);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Should().BeOfType<OkResult>();
            _mockResponse.Verify(x => x.OnCompleted(It.IsAny<Func<object, Task>>(), It.IsAny<object>()), Times.Never);
        }
    }

    [Fact]
    public async Task FakePayment_PaymentEvent_PaymentProcessed()
    {
        // Arrange
        var paymentExternalReference = Guid.NewGuid().ToString();
        var payment = new Fixture().Create<Domain.Core.Entities.Payment>();
        _mockPaymentService.Setup(s => s.GetPaymentAsync(It.IsAny<string>(), It.IsAny<PaymentType>()))
            .ReturnsAsync(payment.FromEntityToDto())
            .Verifiable();

        // Act
        var response = await _target.Post(paymentExternalReference);

        // Assert
        using (new AssertionScope())
        {
            response.Should().NotBeNull();
            response.Should().BeOfType<OkResult>();
        }
    }
}
