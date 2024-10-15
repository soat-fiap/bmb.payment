using FIAP.TechChallenge.ByteMeBurger.Application.UseCases.Orders;
using Bmb.Domain.Core.Interfaces;

namespace FIAP.TechChallenge.ByteMeBurger.Application.Test.UseCases.Orders;

[TestSubject(typeof(GetOrderDetailsUseCase))]
public class GetOrderDetailsUseCaseTest
{
    private readonly Mock<IOrderRepository> _orderRepository;
    private readonly Mock<ICustomerRepository> _customerRepository;
    private readonly IGetOrderDetailsUseCase _useCase;

    public GetOrderDetailsUseCaseTest()
    {
        _orderRepository = new Mock<IOrderRepository>();
        _customerRepository = new Mock<ICustomerRepository>();
        _useCase = new GetOrderDetailsUseCase(_orderRepository.Object, _customerRepository.Object);
    }

    [Fact]
    public async Task Execute_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, null);
        _orderRepository.Setup(r => r.GetAsync(orderId)).ReturnsAsync(order);

        // Act
        var result = await _useCase.Execute(orderId);

        // Assert
        using var scope = new AssertionScope();
        result.Should().NotBeNull();
        result.Id.Should().Be(orderId);
        _customerRepository.Verify(c => c.FindByIdAsync(It.IsAny<Guid>()), Times.Never());
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        _orderRepository.Setup(r => r.GetAsync(orderId)).ReturnsAsync((Order?)null);

        // Act
        var result = await _useCase.Execute(orderId);

        // Assert
        result.Should().BeNull();
        _customerRepository.Verify(c => c.FindByIdAsync(It.IsAny<Guid>()), Times.Never());
    }

    [Fact]
    public async Task Execute_ShouldSetCustomer_WhenCustomerExists()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var order = new Order(orderId, new Customer(customerId));
        var customer = new Customer(customerId, "82227621095", "John Doe", "email@gmail.com");

        _orderRepository.Setup(r => r.GetAsync(orderId)).ReturnsAsync(order);
        _customerRepository.Setup(c => c.FindByIdAsync(customerId)).ReturnsAsync(customer);

        // Act
        var result = await _useCase.Execute(orderId);

        // Assert
        result.Should().NotBeNull();
        result.Customer.Should().NotBeNull();
        result.Customer.Should().Be(customer);
    }

    [Fact]
    public async Task Execute_ShouldReturnNull_WhenOrderIdIsEmpty()
    {
        // Act
        var result = await _useCase.Execute(Guid.Empty);

        // Assert
        result.Should().BeNull();
    }
}
