using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AutoFixture;
using Bmb.Orders.Gateway.Repository;
using Bmb.Payment.Domain;
using FluentAssertions;
using FluentAssertions.Execution;
using JetBrains.Annotations;
using Moq;

namespace Bmb.Orders.Gateway.Test.Repository;

[TestSubject(typeof(OrderReplicaRepository))]
public class OrderReplicaRepositoryTest
{
    private readonly Mock<IAmazonDynamoDB> _mockDynamoDb;
    private readonly OrderReplicaRepository _target;
    private readonly Fixture _fixture;

    public OrderReplicaRepositoryTest()
    {
        _fixture = new Fixture();
        _mockDynamoDb = new Mock<IAmazonDynamoDB>();
        _target = new OrderReplicaRepository(_mockDynamoDb.Object);
    }

    [Fact]
    public async Task SaveCopyAsync_Success()
    {
        // Arrange
        var expectedOrder = _fixture.Create<OrderDto>();

        _mockDynamoDb.Setup(c => c.PutItemAsync(It.IsAny<PutItemRequest>(), default))
            .ReturnsAsync(new PutItemResponse { HttpStatusCode = HttpStatusCode.OK })
            .Verifiable();

        // Act
        var func = () => _target.SaveCopyAsync(expectedOrder);

        // Assert
        using (new AssertionScope())
        {
            await func.Should().NotThrowAsync();
        }
    }

    [Fact]
    public async Task GetCopyAsync_Success()
    {
        // Arrange
        var expectedOrder = _fixture.Create<OrderDto>();

        _mockDynamoDb.Setup(c => c.GetItemAsync(It.IsAny<GetItemRequest>(), default))
            .ReturnsAsync(new GetItemResponse
            {
                Item = expectedOrder.ToDynamoDb()
            });

        // Act
        var result = await _target.GetCopyAsync(expectedOrder.Id);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedOrder);
        }
    }
}