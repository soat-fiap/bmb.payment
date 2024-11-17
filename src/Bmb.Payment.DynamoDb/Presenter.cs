using Amazon.DynamoDBv2.Model;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.Domain.ValueObjects;

namespace Bmb.Payment.DynamoDb;

public static class Presenter
{
    internal static Dictionary<string, AttributeValue> ToDynamoDbItem(this Domain.Entities.Payment payment)
    {
        return new Dictionary<string, AttributeValue>
        {
            { "Id", new AttributeValue(payment.Id.Value.ToString()) },
            { "OrderId", new AttributeValue(payment.OrderId.ToString()) },
            { "Status", new AttributeValue(payment.Status.ToString()) },
            { "PaymentType", new AttributeValue(payment.PaymentType.ToString()) },
            { "Amount", new AttributeValue { N = payment.Amount.ToString() } },
            { "ExternalReference", new AttributeValue(payment.ExternalReference) },
            { "Created", new AttributeValue(payment.Created.ToString()) },
            { "Updated", new AttributeValue(payment.Updated.ToString()) },
            { "QrCode", new AttributeValue(payment.QrCode) }
        };
    }

    internal static Domain.Entities.Payment ToDomain(this Dictionary<string, AttributeValue> item)
    {
        return new Domain.Entities.Payment
        {
            Id = new PaymentId(Guid.Parse(item["Id"].S)),
            Status = Enum.Parse<PaymentStatus>(item["Status"].S),
            PaymentType = Enum.Parse<PaymentType>(item["PaymentType"].S),
            OrderId = Guid.Parse(item["OrderId"].S),
            ExternalReference = item["ExternalReference"].S,
            Created = DateTime.Parse(item["Created"].S),    
            Updated = ConvertUpdatedDateTime(item["Updated"].S),
            QrCode = item["QrCode"].S,
            Amount = decimal.Parse(item["Amount"].N)
        };

        DateTime? ConvertUpdatedDateTime(string amount)
        {
            if (DateTime.TryParse(amount, out var result))
            {
                return result;
            }

            return null;
        }
    }
}