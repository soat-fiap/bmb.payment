using Bmb.Domain.Core.Base;
using Bmb.Domain.Core.Interfaces;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.Controllers;
using Bmb.Payment.FakePayment.Gateway;
using Bmb.Payment.MercadoPago.Gateway;
using Bmb.Payment.MercadoPago.Gateway.Configuration;
using FluentAssertions;
using FluentAssertions.Execution;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bmb.Payment.Api.Test;

[TestSubject(typeof(PaymentGatewayFactory))]
public class PaymentGatewayFactoryTest
{
    private readonly IServiceCollection _serviceCollection;

    public PaymentGatewayFactoryTest()
    {
        _serviceCollection = new ServiceCollection();
        _serviceCollection.AddSingleton<MercadoPagoOptions>(_ => new MercadoPagoOptions()
        {
            AccessToken = "YourAccessToken",
            NotificationUrl = "YourNotificationUrl",
            WebhookSecret = "YourWebhookSecret"
        });

        var mockLogger = new Mock<ILogger<MercadoPagoService>>();
        _serviceCollection.AddSingleton(mockLogger.Object);
    }

    [Fact]
    public void Create_MercadoPago_ReturnsCorrectService()
    {
        // Arrange
        _serviceCollection.AddKeyedScoped<IPaymentGateway, MercadoPagoService>(
            $"Payment-{nameof(PaymentType.MercadoPago)}");
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        var factory = new PaymentGatewayFactory(serviceProvider);

        // Act
        var result = factory.Create(PaymentType.MercadoPago);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeOfType<MercadoPagoService>();
        }
    }

    [Fact]
    public void Create_Test_ReturnsCorrectService()
    {
        // Arrange
        _serviceCollection.AddKeyedScoped<IPaymentGateway, FakePaymentGatewayService>(
            $"Payment-{nameof(PaymentType.Test)}");
        var serviceProvider = _serviceCollection.BuildServiceProvider();
        var factory = new PaymentGatewayFactory(serviceProvider);

        // Act
        var result = factory.Create(PaymentType.Test);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeOfType<FakePaymentGatewayService>();
        }
    }

    [Fact]
    public void Create_InvalidPaymentType_ThrowsException()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        var factory = new PaymentGatewayFactory(serviceProvider);
        var func = () => factory.Create((PaymentType)999);

        // Act & Assert
        func.Should().ThrowExactly<DomainException>();
    }
}
