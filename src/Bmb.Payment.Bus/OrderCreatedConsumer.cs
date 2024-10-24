using Bmb.Domain.Core.Events.Integration;
using Bmb.Payment.Core;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bmb.Payment.Bus;

public class OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger,  IOrdersGateway ordersGateway)
    : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        logger.LogInformation("Message processed: {Message}", context.Message);
        await ordersGateway.SaveCopyAsync(context.Message.ToDto());
    }
}