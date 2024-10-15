using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.Api.Auth;
using Bmb.Payment.Controllers.Contracts;
using Bmb.Payment.MercadoPago.Gateway.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bmb.Payment.Api.Controllers;

/// <summary>
/// MercadoPago webhook controller
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Consumes("application/json")]
[AllowAnonymous]
public class NotificationsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<NotificationsController> _logger;

    /// <summary>
    /// Receive payment providers notifications
    /// </summary>
    /// <param name="paymentService"></param>
    /// <param name="logger"></param>
    public NotificationsController(IPaymentService paymentService, ILogger<NotificationsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Mercado Pago Integration endpoint
    /// </summary>
    /// <param name="event"></param>
    /// <param name="xSignature">Mercado Pago x-signature header</param>
    /// <param name="xRequestId">Mercado Pagox-request-id header</param>
    /// <returns></returns>
    [TypeFilter(typeof(MercadoPagoMessageAuthorizationFilter))]
    [HttpPost("mercadopago")]
    public async Task<IActionResult> Post([FromBody] MercadoPagoWebhookEvent @event,
        [FromHeader(Name = "x-signature")] string xSignature,
        [FromHeader(Name = "x-request-id")] string xRequestId)
    {
        _logger.LogInformation("Received MercadoPago webhook event {@Payload}", @event);
        if (@event.Action == "payment.updated")
        {
            Response.OnCompleted(async () =>
            {
                using (_logger.BeginScope("Processing payment {PaymentId}", @event.Data.Id))
                {
                    await _paymentService.SyncPaymentStatusWithGatewayAsync(@event.Data.Id, PaymentType.MercadoPago);
                }
            });
        }

        return Ok();
    }

    /// <summary>
    /// Fake payment Integration endpoint
    /// </summary>
    [HttpPost("fakepayment")]
    public async Task<IActionResult> Post([FromBody] string externalReference)
    {
        _logger.LogInformation("Received FakePayment webhook event {ExternalReference}", externalReference);

        Response.OnCompleted(async () =>
        {
            await _paymentService.SyncPaymentStatusWithGatewayAsync(externalReference, PaymentType.Test);
        });

        return Ok();
    }
}
