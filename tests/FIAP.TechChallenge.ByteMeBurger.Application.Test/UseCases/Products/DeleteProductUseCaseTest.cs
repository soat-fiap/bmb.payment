using FIAP.TechChallenge.ByteMeBurger.Application.UseCases.Products;

namespace FIAP.TechChallenge.ByteMeBurger.Application.Test.UseCases.Products;

[TestSubject(typeof(DeleteProductUseCase))]
public class DeleteProductUseCaseTest : BaseProductsUseCaseTests
{
    [Fact]
    public async Task DeleteProduct_ShouldCallRepositoryDelete()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productRepository.Setup(repo => repo.DeleteAsync(productId))
            .ReturnsAsync(true);

        // Act
        await _deleteProductUseCase.Execute(productId);

        // Assert
        _productRepository.Verify(repo => repo.DeleteAsync(productId), Times.Once);
    }
}
