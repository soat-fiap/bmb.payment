using AutoFixture;
using AutoFixture.Xunit2;
using FIAP.TechChallenge.ByteMeBurger.Application.UseCases.Orders;
using Bmb.Domain.Core.Base;
using Bmb.Domain.Core.Interfaces;

namespace FIAP.TechChallenge.ByteMeBurger.Application.Test.UseCases.Orders;

[TestSubject(typeof(CreateOrderUseCase))]
public class CreateOrderUseCaseTest
{
    private readonly Mock<ICustomerRepository> _customerRepository;
    private readonly Mock<IProductRepository> _productRepository;
    private readonly ICreateOrderUseCase _useCase;
    private readonly Mock<IOrderTrackingCodeService> _createOrderCodeService;
    private readonly Cpf _validCpf = new("863.917.790-23");
    private readonly OrderTrackingCode _trackingCode = new ("code");

    public CreateOrderUseCaseTest()
    {
        _customerRepository = new Mock<ICustomerRepository>();
        _productRepository = new Mock<IProductRepository>();
        _createOrderCodeService = new Mock<IOrderTrackingCodeService>();
        _useCase = new CreateOrderUseCase(_productRepository.Object, _createOrderCodeService.Object);
    }

    [Theory]
    [AutoData]
    public async Task Checkout_Success(SelectedProduct selectedProduct)
    {
        // Arrange
        var product = new Product(selectedProduct.ProductId, "product", "description", ProductCategory.Drink, 10, []);
        var expectedCustomer = new Customer(Guid.NewGuid(), _validCpf, "customer", "customer@email.com");
        var expectedOrder = new Order(expectedCustomer);
        expectedOrder.SetTrackingCode(_trackingCode);
        expectedOrder.AddOrderItem(selectedProduct.ProductId, product.Name, product.Price, selectedProduct.Quantity);

        _customerRepository.Setup(r => r.FindByCpfAsync(
                It.Is<string>(cpf => cpf == _validCpf.Value)))
            .ReturnsAsync(expectedCustomer);

        _productRepository.Setup(r => r.FindByIdAsync(
                It.IsAny<Guid>()))
            .ReturnsAsync(product);

        _createOrderCodeService.Setup(s => s.GetNext())
            .Returns(new Fixture().Create<OrderTrackingCode>())
            .Verifiable();

        // Act
        var result = await _useCase.Execute(expectedCustomer, [selectedProduct]);

        // Assert
        using (new AssertionScope())
        {
            _productRepository.Verify(m => m.FindByIdAsync(
                It.IsAny<Guid>()), Times.AtLeastOnce);
        }
    }

    [Theory(Skip = "user info comes from bearer token")]
    [InlineAutoData]
    public async Task Checkout_CustomerNotFound_Error(List<SelectedProduct> selectedProducts)
    {
        // Arrange
        var expectedCustomer = new Customer(Guid.NewGuid(), _validCpf, "customer", "customer@email.com");
        var expectedOrder = new Order(expectedCustomer);
        expectedOrder.SetTrackingCode(_trackingCode);
        selectedProducts.ForEach(i => { expectedOrder.AddOrderItem(i.ProductId, "productName", 1, i.Quantity); });

        _customerRepository.Setup(r => r.FindByCpfAsync(
                It.Is<string>(cpf => cpf == _validCpf.Value)))
            .ReturnsAsync(default(Customer));

        // Act
        var func = async () => await _useCase.Execute(expectedCustomer, selectedProducts);

        // Assert
        using (new AssertionScope())
        {
            (await func.Should().ThrowExactlyAsync<EntityNotFoundException>())
                .And
                .Message
                .Should()
                .Be("Customer not found.");

            _customerRepository.Verify(m => m.FindByCpfAsync(
                It.IsAny<string>()), Times.Once);

            _productRepository.Verify(m => m.FindByIdAsync(
                It.IsAny<Guid>()), Times.Never);
        }
    }

    [Theory]
    [InlineAutoData]
    public async Task Checkout_ProductNotFound_Error(List<SelectedProduct> selectedProducts)
    {
        // Arrange
        var expectedCustomer = new Customer(Guid.NewGuid(), _validCpf, "customer", "customer@email.com");
        var expectedOrder = new Order(expectedCustomer);
        selectedProducts.ForEach(i => { expectedOrder.AddOrderItem(i.ProductId, "productName", 2, i.Quantity); });

        _customerRepository.Setup(r => r.FindByCpfAsync(
                It.Is<string>(cpf => cpf == _validCpf.Value)))
            .ReturnsAsync(expectedCustomer);

        _productRepository.Setup(r => r.FindByIdAsync(
                It.IsAny<Guid>()))
            .ReturnsAsync(default(Product));

        // Act
        var func = async () => await _useCase.Execute(expectedCustomer, selectedProducts);

        // Assert
        using (new AssertionScope())
        {
            (await func.Should().ThrowExactlyAsync<EntityNotFoundException>())
                .And
                .Message
                .Should()
                .Be($"Product '{selectedProducts.First().ProductId}' not found.");

            _productRepository.Verify(m => m.FindByIdAsync(
                It.IsAny<Guid>()), Times.Once);
        }
    }
}
