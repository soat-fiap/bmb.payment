using Bmb.Domain.Core.Entities;
using Bmb.Domain.Core.Interfaces;
using Bmb.Domain.Core.ValueObjects;
using FIAP.TechChallenge.ByteMeBurger.Application.UseCases.Customers;

namespace FIAP.TechChallenge.ByteMeBurger.Application.Test.UseCases.Customers;

[TestSubject(typeof(FindCustomerByCpfUseCase))]
public class FindCustomerByCpfUseCaseTest
{
    private readonly Mock<ICustomerRepository> _repository;
    private readonly IFindCustomerByCpfUseCase _useCase;
    private readonly Cpf _validCpf = new("863.917.790-23");

    public FindCustomerByCpfUseCaseTest()
    {
        _repository = new Mock<ICustomerRepository>();
        _useCase = new FindCustomerByCpfUseCase(_repository.Object);
    }

    [Fact]
    public async Task FindBy_Cpf_Success()
    {
        // Arrange
        var expectedCustomer = new Customer(_validCpf, "name", "email@email.com");
        _repository.Setup(r => r.FindByCpfAsync(_validCpf))
            .ReturnsAsync(expectedCustomer);

        // Act
        var customer = await _useCase.Execute(_validCpf);

        // Assert
        using (new AssertionScope())
        {
            customer.Should().NotBeNull();
            customer.Should().BeEquivalentTo(expectedCustomer, options => options.ComparingByMembers<Customer>());
        }
    }

    [Fact]
    public async Task FindBy_Cpf_NotFound()
    {
        // Arrange
        // Act
        var customer = await _useCase.Execute(_validCpf);

        // Assert
        customer.Should().BeNull();
    }
}
