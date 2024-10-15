namespace Bmb.Payment.MySql.Dto;

public class PaymentDto
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public int Status { get; init; }
    public int PaymentType { get; init; }
    public string QrCode { get; set; }
    public decimal Amount { get; init; }
    public DateTime Created { get; init; }
    public DateTime? Updated { get; init; }
    public string ExternalReference { get; set; }

    public PaymentDto()
    {
    }

    public PaymentDto(
        Guid id,
        Guid orderId,
        int status,
        int paymentType,
        decimal amount,
        string externalReference,
        DateTime created,
        DateTime? updated)
    {
        Id = id;
        OrderId = orderId;
        Status = status;
        PaymentType = paymentType;
        Amount = amount;
        ExternalReference = externalReference;
        Created = created;
        Updated = updated;
    }
}
