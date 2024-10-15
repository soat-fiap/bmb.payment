namespace Bmb.Payment.MySql;

internal static class Constants
{
    internal const string InsertPaymentQuery =
        "insert into Payments (Id, OrderId, Status, ExternalReference, Created, PaymentType, Amount) " +
        "values (@Id, @OrderId, @Status, @ExternalReference, @Created, @PaymentType, @Amount);";

    internal const string GetPaymentQuery = "select * from Payments where Id = @Id;";

    internal const string GetPaymentByExternalReferenceQuery =
        "select * from Payments where PaymentType = @PaymentType and ExternalReference = @ExternalReference;";

    internal const string UpdatePaymentStatusQuery =
        "UPDATE Payments SET Status=@Status, Updated=@Updated WHERE Id = @Id";
    
}