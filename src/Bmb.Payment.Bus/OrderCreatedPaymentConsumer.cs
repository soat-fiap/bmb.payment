using Bmb.Domain.Core.Events.Integration;
using Bmb.Payment.Core;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Bmb.Payment.Bus;

public class OrderCreatedPaymentConsumer(ILogger<OrderCreatedPaymentConsumer> logger, IOrdersGateway ordersGateway)
    : IConsumer<OrderCreated>
{
    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        using var scope = logger.BeginScope("Processing {Message} for {OrderCode} {OrderId}",
            context.Message, context.Message.OrderTrackingCode, context.Message.Id);
        {
            await ordersGateway.SaveCopyAsync(context.Message.ToDto());
            logger.LogInformation("Message processed: {Message}", context.Message);
        }
    }
}