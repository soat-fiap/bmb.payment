using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.Core.Contracts;

namespace Bmb.Payment.MySql.Repository;

public class PaymentRepository(IAmazonDynamoDB database) : IPaymentRepository
{
    private const string PaymentTable = "Payments";

    public async Task<Domain.Core.Entities.Payment> SaveAsync(Domain.Core.Entities.Payment payment)
    {
        await database.PutItemAsync(PaymentTable, payment.ToDynamoDbItem());
        return payment;
    }

    public async Task<Domain.Core.Entities.Payment?> GetPaymentAsync(PaymentId paymentId)
    {
        var request = new GetItemRequest
        {
            TableName = PaymentTable,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = paymentId.Value.ToString() } }
            }
        };

        var response = await database.GetItemAsync(request);

        if (response.Item == null || response.Item.Count == 0)
        {
            return null;
        }

        return response.Item.ToDomain();
    }

    public async Task<Domain.Core.Entities.Payment?> GetPaymentAsync(string externalReference, PaymentType paymentType)
    {
        var request = new QueryRequest
        {
            TableName = PaymentTable,
            IndexName = "ExternalReference-index",
            KeyConditionExpression = "ExternalReference = :externalReference",
            FilterExpression = "PaymentType = :paymentType",
            ExpressionAttributeValues = new Dictionary<string, AttributeValue>
            {
                { ":externalReference", new AttributeValue(externalReference) },
                { ":paymentType", new AttributeValue(paymentType.ToString()) }
            }
        };
        var response = await database. QueryAsync(request);
        if (response.Items is null || response.Items.Count == 0)
        {
            return null;
        }

        return response.Items[0].ToDomain();
    }

    public async Task<bool> UpdatePaymentStatusAsync(Domain.Core.Entities.Payment payment)
    {
        var attributes = payment.ToDynamoDbItem();
        attributes.Remove("Id");
        
        var request = new UpdateItemRequest
        {
            TableName =PaymentTable,
            Key = new Dictionary<string, AttributeValue>
            {
                { "Id", new AttributeValue { S = payment.Id.Value.ToString() } }
            },
            AttributeUpdates = attributes.ToDictionary(x => x.Key, x => new AttributeValueUpdate { Action = AttributeAction.PUT, Value = x.Value })
        };

        var response = await database.UpdateItemAsync(request);
        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}