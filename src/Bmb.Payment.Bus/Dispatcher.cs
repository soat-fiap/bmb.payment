using Bmb.Domain.Core.Events.Notifications;
using Bmb.Payment.Core.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bmb.Payment.Bus;

public class Dispatcher : IDispatcher
{
    private readonly IBus _bus;
    private readonly ILogger<Dispatcher> _logger;

    public Dispatcher(IBus bus, ILogger<Dispatcher> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : IBmbNotification
    {
        try
        {
            _logger.LogInformation("Publishing event: {Event}", @event);
            await _bus.Publish(@event, cancellationToken);
            _logger.LogInformation("Event published: {Event}", @event);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when trying to publish event: {Event}", @event);
            throw;
        }
    }
}