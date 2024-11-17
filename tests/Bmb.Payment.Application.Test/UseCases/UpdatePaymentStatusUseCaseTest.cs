using AutoFixture;
using Bmb.Domain.Core.Events;
using Bmb.Domain.Core.Events.Notifications;
using Bmb.Payment.Application.UseCases;
using Bmb.Payment.Domain.Contracts;
using Bmb.Payment.Domain.ValueObjects;

namespace Bmb.Payment.Application.Test.UseCases;

[TestSubject(typeof(UpdatePaymentStatusUseCase))]
public class UpdatePaymentStatusUseCaseTest
{
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly UpdatePaymentStatusUseCase _target;
     private readonly Mock<IDispatcher> _dispatcher;

    public UpdatePaymentStatusUseCaseTest()
    {
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _dispatcher = new Mock<IDispatcher>();
        _target = new UpdatePaymentStatusUseCase(_mockPaymentRepository.Object , _dispatcher.Object);
    }

    [Fact]
    public async void Execute_UpdatePaymentAndOrderStatus_Success()
    {
        // Arrange
        var payment = new Fixture().Create<Domain.Entities.Payment>();
        var newStatus = PaymentStatus.Approved;

        _mockPaymentRepository.Setup(p =>
                p.UpdatePaymentStatusAsync(It.Is<Domain.Entities.Payment>(x => x.Status == newStatus)))
            .ReturnsAsync(true)
            .Verifiable();

        // Act
        var result = await _target.Execute(payment, newStatus);

        // Assert
        using (new AssertionScope())
        {
            result.Should().BeTrue();
            _dispatcher.VerifyAll();
            _dispatcher.Verify(d => d.PublishAsync(It.IsAny<OrderPaymentConfirmed>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
