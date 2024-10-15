using System.Collections.ObjectModel;
using AutoFixture.Xunit2;
using FIAP.TechChallenge.ByteMeBurger.Application.UseCases.Products;

namespace FIAP.TechChallenge.ByteMeBurger.Application.Test.UseCases.Products;

[TestSubject(typeof(FindProductsByCategoryUseCase))]
public class FindProductsByCategoryUseCaseTest: BaseProductsUseCaseTests
{
    [Fact]
    public async Task FindByCategory_NotFound()
    {
        // Arrange
        _productRepository.Setup(s => s.FindByCategory(It.IsAny<ProductCategory>()))
            .ReturnsAsync((ReadOnlyCollection<Product>)null!);

        // Act
        var products = await _findProductsByCategoryUseCase.Execute(ProductCategory.Drink);

        // Assert
        using (new AssertionScope())
        {
            products.Should().NotBeNull();
            products.Should().BeEmpty();
            _productRepository.VerifyAll();
        }
    }

    [Theory]
    [InlineAutoData]
    public async Task FindByCategory_Success(ProductCategory category)
    {
        // Arrange
        var expectedProducts = new List<Product>()
        {
            new Product(Guid.NewGuid(), "product", "description", category, 10m, [])
        };

        _productRepository.Setup(s => s.FindByCategory(It.Is<ProductCategory>(c => c == category)))
            .ReturnsAsync(expectedProducts.AsReadOnly);

        // Act
        var products = await _findProductsByCategoryUseCase.Execute(category);

        // Assert
        using (new AssertionScope())
        {
            products.Should().BeEquivalentTo(expectedProducts);
            _productRepository.VerifyAll();
        }
    }
}
