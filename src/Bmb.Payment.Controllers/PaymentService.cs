using Bmb.Domain.Core.Interfaces;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.Application.UseCases;
using Bmb.Payment.Controllers.Contracts;
using Bmb.Payment.Controllers.Dto;

namespace Bmb.Payment.Controllers;

public class PaymentService : IPaymentService
{
    private readonly ICreatePaymentUseCase _createOrderPaymentUseCase;
    private readonly IUpdatePaymentStatusUseCase _updatePaymentStatusUseCase;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentGatewayFactoryMethod _paymentGatewayFactory;

    public PaymentService(ICreatePaymentUseCase createOrderPaymentUseCase,
        IUpdatePaymentStatusUseCase updatePaymentStatusUseCase,
        IPaymentRepository paymentRepository,
        IPaymentGatewayFactoryMethod paymentGatewayFactory)
    {
        _createOrderPaymentUseCase = createOrderPaymentUseCase;
        _updatePaymentStatusUseCase = updatePaymentStatusUseCase;
        _paymentRepository = paymentRepository;
        _paymentGatewayFactory = paymentGatewayFactory;
    }

    public async Task<PaymentDto> CreateOrderPaymentAsync(CreateOrderPaymentRequestDto command)
    {
        var payment = await _createOrderPaymentUseCase.Execute(command.OrderId, command.PaymentType);
        if (payment is null)
            return null;

        await _paymentRepository.SaveAsync(payment);
        // TODO MassTransit
        // await _updateOrderPaymentUseCase.Execute(payment.OrderId, payment.Id);
        return payment.FromEntityToDto();
    }

    public async Task<PaymentDto?> GetPaymentAsync(Guid id)
    {
        var payment = await _paymentRepository.GetPaymentAsync(new PaymentId(id));
        return payment?.FromEntityToDto();
    }

    public async Task<PaymentDto?> GetPaymentAsync(string externalReference, PaymentType paymentType)
    {
        var payment = await _paymentRepository.GetPaymentAsync(externalReference, paymentType);
        return payment?.FromEntityToDto();
    }

    public async Task<bool> SyncPaymentStatusWithGatewayAsync(string externalReference, PaymentType paymentType)
    {
        var payment = await _paymentRepository.GetPaymentAsync(externalReference, paymentType);
        if (payment is null)
            return false;

        var paymentGateway = _paymentGatewayFactory.Create(paymentType);
        var paymentStatus = await paymentGateway.GetPaymentStatusAsync(externalReference);
        if (paymentStatus is null)
            return false;

        await _updatePaymentStatusUseCase.Execute(payment, paymentStatus.Value);
        return true;
    }
}
