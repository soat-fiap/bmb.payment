using AutoFixture;
using FIAP.TechChallenge.ByteMeBurger.Application.UseCases.Products;
using FIAP.TechChallenge.ByteMeBurger.Test.Common;

namespace FIAP.TechChallenge.ByteMeBurger.Application.Test.UseCases.Products;

[TestSubject(typeof(UpdateProductUseCase))]
public class UpdateProductUseCaseTest : BaseProductsUseCaseTests
{
    [Fact]
    public async Task Update_NotFound()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Customizations.Add(new ProductGenerator());
        var product = fixture.Create<Product>();

        _productRepository.Setup(s => s.FindByIdAsync(
                It.IsAny<Guid>()))
            .ReturnsAsync((Product?)null);

        // Act
        var updated = await _updateProductUseCase.Execute(product.Id, product.Name, product.Description,
            ProductCategory.Drink, product.Price, product.Images);

        // Assert
        using (new AssertionScope())
        {
            updated.Should().BeFalse();
            _productRepository.Verify(m => m.UpdateAsync(It.IsAny<Product>()), Times.Never);
            _productRepository.VerifyAll();
        }
    }

    [Fact]
    public async Task Update_Fail()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Customizations.Add(new ProductGenerator());
        var product = fixture.Create<Product>();

        _productRepository.Setup(s => s.FindByIdAsync(
                It.IsAny<Guid>()))
            .ReturnsAsync(product);

        _productRepository.Setup(s => s.UpdateAsync(
                It.IsAny<Product>()))
            .ReturnsAsync(false);

        // Act
        var updated = await _updateProductUseCase.Execute(product.Id, product.Name, product.Description,
            ProductCategory.Drink, product.Price, product.Images);

        // Assert
        using (new AssertionScope())
        {
            updated.Should().BeFalse();
            _productRepository.VerifyAll();
        }
    }

    [Fact]
    public async Task Update_Success()
    {
        // Arrange
        var fixture = new Fixture();
        fixture.Customizations.Add(new ProductGenerator());
        var product = fixture.Create<Product>();

        _productRepository.Setup(s => s.FindByIdAsync(
                It.IsAny<Guid>()))
            .ReturnsAsync(product);

        _productRepository.Setup(s => s.UpdateAsync(
                It.IsAny<Product>()))
            .ReturnsAsync(true);

        // Act
        var updated = await _updateProductUseCase.Execute(product.Id, product.Name, product.Description,
            ProductCategory.Drink, product.Price, product.Images);

        // Assert
        using (new AssertionScope())
        {
            updated.Should().BeTrue();
            _productRepository.VerifyAll();
        }
    }
}
