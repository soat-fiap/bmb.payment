using FIAP.TechChallenge.ByteMeBurger.Application.UseCases.Orders;
using Bmb.Domain.Core.Base;
using Bmb.Domain.Core.Interfaces;

namespace FIAP.TechChallenge.ByteMeBurger.Application.Test.UseCases.Orders;

[TestSubject(typeof(UpdateOrderStatusUseCase))]
public class UpdateOrderStatusUseCaseTest
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly UpdateOrderStatusUseCase _useCase;

    public UpdateOrderStatusUseCaseTest()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _useCase = new UpdateOrderStatusUseCase(_orderRepositoryMock.Object);
    }

    [Theory]
    [InlineData(OrderStatus.PaymentPending, OrderStatus.Received)]
    [InlineData(OrderStatus.Received, OrderStatus.InPreparation)]
    [InlineData(OrderStatus.InPreparation, OrderStatus.Ready)]
    [InlineData(OrderStatus.Ready, OrderStatus.Completed)]
    public async Task Execute_ShouldUpdateOrderStatus_WhenOrderExists(OrderStatus currentStatus, OrderStatus newStatus)
    {
        // Arrange
        var currentOrder = new Order(Guid.NewGuid(), null, currentStatus, new OrderTrackingCode("code"),
            DateTime.UtcNow, null);
        _orderRepositoryMock.Setup(r => r.GetAsync(currentOrder.Id)).ReturnsAsync(currentOrder);
        _orderRepositoryMock.Setup(r => r.UpdateOrderStatusAsync(It.IsAny<Order>()))
            .ReturnsAsync(true);

        // Act
        var updated = await _useCase.Execute(currentOrder.Id, newStatus);

        // Assert
        using (new AssertionScope())
        {
            _orderRepositoryMock.Verify(r => r.GetAsync(It.Is<Guid>(orderId => orderId == currentOrder.Id)),
                Times.Once);
            _orderRepositoryMock.Verify(
                r => r.UpdateOrderStatusAsync(It.Is<Order>(orderId => orderId.Status == newStatus)),
                Times.Once);
        }

        updated.Should().BeTrue();
    }

    [Fact]
    public async Task Execute_ShouldThrowException_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetAsync(orderId)).ReturnsAsync((Order)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _useCase.Execute(orderId, OrderStatus.Completed));
    }

    [Fact]
    public async Task Execute_ShouldThrowException_WhenOrderIsAlreadyCompleted()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _orderRepositoryMock.Setup(r => r.GetAsync(orderId))
            .ReturnsAsync(new Order(Guid.NewGuid(), null, OrderStatus.Completed, new OrderTrackingCode("orderCode"),
                DateTime.UtcNow, null));

        // Act & Assert
        await Assert.ThrowsAsync<DomainException>(() => _useCase.Execute(orderId, OrderStatus.Completed));
    }
}
