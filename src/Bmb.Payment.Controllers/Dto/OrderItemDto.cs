namespace Bmb.Payment.Controllers.Dto;

/// <summary>
/// Product added to order
/// </summary>
public class OrderItemDto
{
    /// <summary>
    /// Order Id
    /// </summary>
    public Guid OrderId { get; set; }

    /// <summary>
    /// Product Id
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Quantity
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Unit price
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Product name
    /// </summary>
    public string ProductName { get; set; } = string.Empty;
}
