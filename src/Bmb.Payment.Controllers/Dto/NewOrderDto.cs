namespace Bmb.Payment.Controllers.Dto;

/// <summary>
/// Newly created order
/// </summary>
/// <param name="Id">Order internal Id. </param>
/// <param name="TrackingCode">Tracking code. </param>
public record NewOrderDto(Guid Id, string TrackingCode);
