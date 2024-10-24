using Bmb.Domain.Core.Events.Notifications;
using FluentAssertions;
using JetBrains.Annotations;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using static Moq.It;

namespace Bmb.Payment.Bus.Test
{
    [TestSubject(typeof(Dispatcher))]
    public class DispatcherTest
    {
        private readonly Mock<IBus> _busMock;
        private readonly Mock<ILogger<Dispatcher>> _loggerMock;
        private readonly Dispatcher _dispatcher;

        public DispatcherTest()
        {
            _busMock = new Mock<IBus>();
            _loggerMock = new Mock<ILogger<Dispatcher>>();
            _dispatcher = new Dispatcher(_busMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task PublishAsync_ShouldPublishEvent_WhenCalled()
        {
            // Arrange
            var @event = new Mock<IBmbNotification>().Object;

            // Act
            Func<Task> act = async () => await _dispatcher.PublishAsync(@event);

            // Assert
            await act.Should().NotThrowAsync();
            _busMock.Verify(b => b.Publish(IsAny<object>(), IsAny<CancellationToken>()), Times.Once);
            _loggerMock.VerifyLog(logger => logger.LogInformation(IsAny<string>(), @event), LogLevel.Information,
                Times.Exactly(2));
        }

        [Fact]
        public async Task PublishAsync_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var @event = new Mock<IBmbNotification>().Object;
            var exception = new Exception("Test exception");
            _busMock.Setup(b => b.Publish(IsAny<object>(), IsAny<CancellationToken>())).ThrowsAsync(exception);

            // Act
            Func<Task> act = async () => await _dispatcher.PublishAsync(@event);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Test exception");
            _loggerMock.VerifyLog(logger => logger.LogError(IsAny<string>(), @event), LogLevel.Error, Times.Once());
        }
    }

    public static class LoggerExtensions
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> loggerMock, Action<ILogger<T>> verify, LogLevel logLevel,
            Times times)
        {
            loggerMock.Verify(
                x => x.Log(
                    logLevel,
                    0,
                    IsAny<IsAnyType>(),
                    IsAny<Exception>(),
                    IsAny<Func<IsAnyType, Exception?, string>>()),
                times);
        }
    }
}