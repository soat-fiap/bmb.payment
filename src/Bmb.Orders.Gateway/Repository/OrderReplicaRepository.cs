using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Bmb.Domain.Core.Entities;
using Bmb.Payment.Core;

namespace Bmb.Orders.Gateway.Repository;

internal class OrderReplicaRepository(IAmazonDynamoDB database) : IOrdersGateway
{
    private const string OrdersReplicaTable = "Payment-OrdersReplica";

    public async Task<OrderDto?> GetCopyAsync(Guid orderId)
    {
        var order = default(OrderDto);
        var response = await database.GetItemAsync(new GetItemRequest(OrdersReplicaTable,
            new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue(orderId.ToString()) }
            }));

        if (response?.Item is null || response.Item.Count == 0)
        {
            return order;
        }

        return response.Item.ToDomain();
    }

    public Task SaveCopyAsync(OrderDto order)
    {
        return database.PutItemAsync(OrdersReplicaTable, order.ToDynamoDb());
    }
}