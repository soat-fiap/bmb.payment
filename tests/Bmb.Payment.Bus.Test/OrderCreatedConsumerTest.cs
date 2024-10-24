using AutoFixture;
using Bmb.Domain.Core.Events.Integration;
using Bmb.Payment.Core;
using JetBrains.Annotations;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Bmb.Payment.Bus.Test;

[TestSubject(typeof(OrderCreatedConsumer))]
public class OrderCreatedConsumerTest
{
    private readonly Mock<ILogger<OrderCreatedConsumer>> _loggerMock;
    private readonly Mock<IOrdersGateway> _ordersGatewayMock;
    private readonly OrderCreatedConsumer _consumer;

    public OrderCreatedConsumerTest()
    {
        _loggerMock = new Mock<ILogger<OrderCreatedConsumer>>();
        _ordersGatewayMock = new Mock<IOrdersGateway>();
        _consumer = new OrderCreatedConsumer(_loggerMock.Object, _ordersGatewayMock.Object);
    }

    [Fact]
    public async Task Consume_ShouldLogInformationAndSaveCopy_WhenCalled()
    {
        // Arrange
        var orderCreated = new Fixture().Create<OrderCreated>();
        var contextMock = new Mock<ConsumeContext<OrderCreated>>();
        contextMock.Setup(c => c.Message).Returns(orderCreated);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _loggerMock.VerifyLog(logger => logger.LogInformation("Message processed: {Message}", orderCreated),
            LogLevel.Information, Times.Once());
        _ordersGatewayMock.Verify(gateway => gateway.SaveCopyAsync(It.IsAny<OrderDto>(), default), Times.Once);
    }
}