using Bmb.Domain.Core.ValueObjects;
using FIAP.TechChallenge.ByteMeBurger.Application.DomainServices;

namespace FIAP.TechChallenge.ByteMeBurger.Application.Test.DomainServices;

[TestSubject(typeof(OrderTrackingCodeService))]
public class OrderTrackingCodeServiceTest
{
    private readonly OrderTrackingCodeService _orderTrackingCodeService = new ();

    [Fact]
    public void GetNextOrderCode_Success()
    {
        // Arrange & Act
        var code = _orderTrackingCodeService.GetNext();

        // Assert
        using (new AssertionScope())
        {
            code.Should().NotBeNull();
            code.Value.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public async void GetNextOrderCodeAsync_Success()
    {
        // Arrange & Act
        var code = await _orderTrackingCodeService.GetNextAsync();

        // Assert
        using (new AssertionScope())
        {
            code.Should().NotBeNull();
            code.Value.Should().NotBeNullOrWhiteSpace();
        }
    }

    [Fact]
    public void Multiple_GetNextOrderCode_Success()
    {
        // Arrange
        var list = new List<OrderTrackingCode>();

        // Act
        for (var i = 0; i < 10; i++)
        {
            list.Add(_orderTrackingCodeService.GetNext());
        }

        // Assert
        using (new AssertionScope())
        {
            list.Should().OnlyHaveUniqueItems();
        }
    }
}
