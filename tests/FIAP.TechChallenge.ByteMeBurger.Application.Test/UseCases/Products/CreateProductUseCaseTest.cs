using AutoFixture.Xunit2;
using FIAP.TechChallenge.ByteMeBurger.Application.UseCases.Products;

namespace FIAP.TechChallenge.ByteMeBurger.Application.Test.UseCases.Products;

[TestSubject(typeof(CreateProductUseCase))]
public class CreateProductUseCaseTest : BaseProductsUseCaseTests
{

    [Theory]
    [InlineAutoData]
    public async Task Create_Product_Success(string name, string description, ProductCategory category, decimal price,
        string[] images)
    {
        // Arrange
        var expectedProduct = new Product(name, description, category, price, images);
        expectedProduct.Create();

        _productRepository.Setup(s => s.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync(expectedProduct);

        // Act
        var product = await _createProductUseCase.Execute(name, description, category, price, images);

        // Assert
        using (new AssertionScope())
        {
            product.Should().NotBeNull();
            product.Id.Should().Be(expectedProduct.Id);
            product.Name.Should().Be(expectedProduct.Name);
            product.Description.Should().Be(expectedProduct.Description);
            product.Category.Should().Be(category);
            product.Price.Should().Be(price);
            product.Images.Should().BeEquivalentTo(images);
            product.Created.Should().NotBe(default);
            product.Updated.Should().BeNull();
            _productRepository.VerifyAll();
        }
    }
}
