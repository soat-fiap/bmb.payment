using Bmb.Domain.Core.Events.Notifications;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Gherkin.Quick;

namespace Bmb.Payment.Masstransit.Test.Gherkin;

[FeatureFile("./Gherkin/Dispatcher.feature")]
public class DispatcherSteps : Feature
{
    private readonly Mock<IBus> _busMock;
    private readonly Mock<ILogger<Dispatcher>> _loggerMock;
    private readonly Dispatcher _dispatcher;
    private IBmbNotification _event;
    private Exception _caughtException;

    public DispatcherSteps()
    {
        _busMock = new Mock<IBus>();
        _loggerMock = new Mock<ILogger<Dispatcher>>();

        _dispatcher = new Dispatcher(_busMock.Object, _loggerMock.Object);
    }

    [Given("a valid event")]
    public void GivenAValidEvent()
    {
        _event = new Mock<IBmbNotification>().Object;
    }

    [And("the bus throws an exception")]
    public void GivenTheBusThrowsAnException()
    {
        _busMock.Setup(b => b.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test exception"));
    }

    [When("PublishAsync is called")]
    public async Task WhenPublishAsyncIsCalled()
    {
        try
        {
            await _dispatcher.PublishAsync(_event, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _caughtException = ex;
        }
    }

    [Then("it should publish the event")]
    public void ThenItShouldPublishTheEvent()
    {
        _busMock.Verify(b => b.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [And("it should log the event information")]
    public void ThenItShouldLogTheEventInformation()
    {
        _loggerMock.Verify(logger =>
            logger.Log(LogLevel.Information, 0, It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(2));
    }


    [Then("it should log the initial event information")]
    public void ThenItShouldLogTheInitialEventInformation()
    {
        _loggerMock.Verify(logger =>
            logger.Log(LogLevel.Information, 0, It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [And("it should log the error")]
    public void ThenItShouldLogTheError()
    {
        _loggerMock.Verify(logger =>
            logger.Log(LogLevel.Error, 0, It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    [And("it should throw the exception")]
    public void ThenItShouldThrowTheException()
    {
        _caughtException.Should().NotBeNull();
        _caughtException.Message.Should().Be("Test exception");
    }
}