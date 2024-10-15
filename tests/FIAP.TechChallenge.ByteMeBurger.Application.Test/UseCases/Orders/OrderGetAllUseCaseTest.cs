using System.Collections.ObjectModel;
using FIAP.TechChallenge.ByteMeBurger.Application.UseCases.Orders;
using Bmb.Domain.Core.Interfaces;

namespace FIAP.TechChallenge.ByteMeBurger.Application.Test.UseCases.Orders;

[TestSubject(typeof(OrderGetAllUseCase))]
public class OrderGetAllUseCaseTest
{
    private readonly Mock<IOrderRepository> _orderRepository;
    private readonly IOrderGetAllUseCase _useCase;
    private readonly ReadOnlyCollection<Order> _orders;

    public OrderGetAllUseCaseTest()
    {
        _orders = new List<Order>()
        {
            new(Guid.NewGuid(), null, OrderStatus.PaymentPending, null, DateTime.Now, null),
            new(Guid.NewGuid(), null, OrderStatus.InPreparation, null, DateTime.Now, null),
            new(Guid.NewGuid(), null, OrderStatus.Ready, null, DateTime.Now, null),
            new(Guid.NewGuid(), null, OrderStatus.Received, null, DateTime.Now, null),
            new(Guid.NewGuid(), null, OrderStatus.Completed, null, DateTime.Now, null),
        }.AsReadOnly();

        _orderRepository = new Mock<IOrderRepository>();
        _useCase = new OrderGetAllUseCase(_orderRepository.Object);
    }

    [Fact]
    public async Task GetAll_SelectedStatus_Success()
    {
        // Arrange
        var expectedOrders = new List<Order>()
        {
            _orders[2],
            _orders[1],
            _orders[3],
        }.AsReadOnly();

        _orderRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(_orders.AsReadOnly);

        // Act
        var result = await _useCase.Execute(false);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expectedOrders, options => options.WithStrictOrdering());
            _orderRepository.Verify(m => m.GetAllAsync(), Times.Once);
        }
    }

    [Fact]
    public async Task GetAll_Success()
    {
        // Arrange
        var expectedOrders = new List<Order>(_orders)
            .OrderByDescending(o => o.Status)
            .ThenBy(o => o.Created)
            .ToList()
            .AsReadOnly();

        _orderRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync(_orders.AsReadOnly);

        // Act
        var result = await _useCase.Execute(true);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().BeEquivalentTo(expectedOrders, options => options.WithStrictOrdering());
            _orderRepository.Verify(m => m.GetAllAsync(), Times.Once);
        }
    }

    [Fact]
    public async Task GetAll_Empty()
    {
        // Arrange
        _orderRepository.Setup(r => r.GetAllAsync())
            .ReturnsAsync((ReadOnlyCollection<Order>)default!);

        // Act
        var result = await _useCase.Execute(true);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _orderRepository.Verify(m => m.GetAllAsync(), Times.Once);
        }
    }
}
