using System.Net;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using AutoFixture;
using Bmb.Payment.MySql.Repository;
using FluentAssertions;
using FluentAssertions.Execution;
using JetBrains.Annotations;
using Moq;

namespace Bmb.Payment.MySql.Test.Repository;

[TestSubject(typeof(PaymentRepository))]
public class PaymentRepositoryTest
{
    private readonly Mock<IAmazonDynamoDB> _mockDynamoDb;
    private readonly PaymentRepository _target;

    public PaymentRepositoryTest()
    {
        _mockDynamoDb = new Mock<IAmazonDynamoDB>();
        _target = new PaymentRepository(_mockDynamoDb.Object);
    }

    [Fact]
    public async Task Create_Success()
    {
        // Arrange
        var expectedPayment = new Fixture().Create<Domain.Core.Entities.Payment>();

        _mockDynamoDb.Setup(c => c.PutItemAsync(It.IsAny<PutItemRequest>(), default))
            .ReturnsAsync(new PutItemResponse { HttpStatusCode = HttpStatusCode.OK });

        // Act
        var result = await _target.SaveAsync(expectedPayment);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().Be(expectedPayment);
        }
    }

    [Fact]
    public async Task GetPaymentAsync_Success()
    {
        // Arrange
        var expectedPayment = new Fixture().Create<Domain.Core.Entities.Payment>();

        _mockDynamoDb.Setup(c => c.GetItemAsync(It.IsAny<GetItemRequest>(), default))
            .ReturnsAsync(new GetItemResponse
            {
                Item = expectedPayment.ToDynamoDbItem()
            });

        // Act
        var result = await _target.GetPaymentAsync(expectedPayment.Id);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Status.Should().Be(expectedPayment.Status);
        }
    }

    [Fact]
    public async Task GetPaymentAsync_ExternalReference_Success()
    {
        // Arrange
        var expectedPayment = new Fixture().Create<Domain.Core.Entities.Payment>();

        _mockDynamoDb.Setup(c => c.ScanAsync(It.IsAny<ScanRequest>(), default))
            .ReturnsAsync(new ScanResponse
            {
                Items = new List<Dictionary<string, AttributeValue>>
                {
                    expectedPayment.ToDynamoDbItem()
                }
            });

        // Act
        var result = await _target.GetPaymentAsync(expectedPayment.ExternalReference, expectedPayment.PaymentType);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Status.Should().Be(expectedPayment.Status);
        }
    }

    [Fact]
    public async Task UpdatePaymentStatusAsync_Success()
    {
        // Arrange
        var payment = new Fixture().Create<Domain.Core.Entities.Payment>();

        _mockDynamoDb.Setup(c => c.UpdateItemAsync(It.IsAny<UpdateItemRequest>(), default))
            .ReturnsAsync(new UpdateItemResponse { HttpStatusCode = HttpStatusCode.OK });

        // Act
        var result = await _target.UpdatePaymentStatusAsync(payment);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeTrue();
        }
    }
}