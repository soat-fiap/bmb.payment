using AutoFixture;
using Bmb.Domain.Core.Base;
using Bmb.Payment.Application.UseCases;
using Bmb.Payment.Core;
using Bmb.Payment.Core.Contracts;

namespace Bmb.Payment.Application.Test.UseCases;

[TestSubject(typeof(CreatePaymentUseCase))]
public class CreatePaymentUseCaseTest
{
    private readonly Mock<IPaymentGateway> _paymentGatewayMock;

    private readonly CreatePaymentUseCase _createPaymentUseCase;

    private readonly Mock<IOrdersGateway> _ordersGateway;

    public CreatePaymentUseCaseTest()
    {
        _paymentGatewayMock = new Mock<IPaymentGateway>();
        Mock<IPaymentGatewayFactoryMethod> paymentGatewayFactory = new();
        _ordersGateway = new Mock<IOrdersGateway>();
        _createPaymentUseCase = new CreatePaymentUseCase(paymentGatewayFactory.Object, _ordersGateway.Object);

        paymentGatewayFactory.Setup(g => g.Create(It.IsAny<PaymentType>()))
            .Returns(_paymentGatewayMock.Object);
    }

    [Fact]
    public async Task Execute_ValidOrder_ShouldReturnPayment()
    {
        // Arrange
        var fixture = new Fixture();
        var order = fixture.Build<OrderDto>()
            .Without(o => o.PaymentId)
            .Create();

        var expectedPayment = fixture.Build<Bmb.Domain.Core.Entities.Payment>()
            .With(p => p.Id, new PaymentId(Guid.NewGuid()))
            .With(p => p.Status, PaymentStatus.Pending)
            .Create();

        _ordersGateway.Setup(o => o.GetCopyAsync(It.IsAny<Guid>()))
            .ReturnsAsync(order);
        _paymentGatewayMock.Setup(ps => ps.CreatePaymentAsync(It.IsAny<OrderDto>()))
            .ReturnsAsync(expectedPayment);

        // Act
        var payment = await _createPaymentUseCase.Execute(Guid.NewGuid(), PaymentType.Test);

        // Assert
        using var scope = new AssertionScope();
        payment.Should().NotBeNull();
        payment.Status.Should().Be(PaymentStatus.Pending);
        _paymentGatewayMock.Verify(ps => ps.CreatePaymentAsync(It.IsAny<OrderDto>()), Times.Once);
    }

    [Fact]
    public async Task Execute_OrderNotFound_ShouldThrowDomainException()
    {
        // Arrange
        _ordersGateway.Setup(o => o.GetCopyAsync(It.IsAny<Guid>()))
            .ReturnsAsync((OrderDto?)default);

        // Act
        var func = () => _createPaymentUseCase.Execute(Guid.NewGuid(), PaymentType.Test);

        // Assert
        using var scope = new AssertionScope();
        await func.Should()
            .ThrowExactlyAsync<EntityNotFoundException>()
            .WithMessage("Order not found.");
        _paymentGatewayMock.Verify(ps => ps.CreatePaymentAsync(It.IsAny<OrderDto>()), Times.Never);
    }

    [Fact]
    public async Task Execute_OrderAlreadyHasPayment_ShouldThrowDomainException()
    {
        // Arrange
        _ordersGateway.Setup(o => o.GetCopyAsync(It.IsAny<Guid>()))
            .ReturnsAsync(new OrderDto(Guid.NewGuid(), null, new List<OrderItemDto>(), OrderStatus.PaymentPending, "",
                new PaymentId(Guid.NewGuid()), 10));

        // Act
        var func = () => _createPaymentUseCase.Execute(Guid.NewGuid(), PaymentType.Test);

        // Assert
        using var scope = new AssertionScope();
        await func.Should()
            .ThrowExactlyAsync<DomainException>()
            .WithMessage("There's already a Payment for the order.");
    }
}