using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Bmb.Domain.Core.Base;
using Bmb.Domain.Core.Entities;
using Bmb.Payment.Domain;

namespace Bmb.Orders.Gateway.Repository;

internal class OrderReplicaRepository(IAmazonDynamoDB database) : IOrdersGateway
{
    private const string OrdersReplicaTable = "Payment-OrdersReplica";

    public async Task<OrderDto?> GetCopyAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var order = default(OrderDto);
            var response = await database.GetItemAsync(new GetItemRequest(OrdersReplicaTable,
                new Dictionary<string, AttributeValue>
                {
                    { "Id", new AttributeValue(orderId.ToString()) }
                }), cancellationToken);

            if (response?.Item is null || response.Item.Count == 0)
            {
                return order;
            }

            return response.Item.ToDomain();
        }
        catch (AmazonDynamoDBException e)
        {
           throw new DomainException("Error getting order from replica", e);
        }
    }

    public Task SaveCopyAsync(OrderDto order, CancellationToken cancellationToken = default)
    {
        return database.PutItemAsync(OrdersReplicaTable, order.ToDynamoDb(), cancellationToken);
    }
}