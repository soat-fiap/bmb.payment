using Bmb.Domain.Core.Base;
using Bmb.Domain.Core.Entities;
using Bmb.Domain.Core.Interfaces;
using Bmb.Domain.Core.ValueObjects;
using FIAP.TechChallenge.ByteMeBurger.Application.UseCases.Customers;

namespace FIAP.TechChallenge.ByteMeBurger.Application.Test.UseCases.Customers;

[TestSubject(typeof(CreateCustomerUseCase))]
public class CreateCustomerUseCaseTest
{
    private readonly Mock<ICustomerRepository> _repository;
    private readonly ICreateCustomerUseCase _useCase;
    private readonly Cpf _validCpf = new("863.917.790-23");

    public CreateCustomerUseCaseTest()
    {
        _repository = new Mock<ICustomerRepository>();
        _useCase = new CreateCustomerUseCase(_repository.Object);
    }

    [Fact]
    public async Task Create_Customer_CpfOnly_Success()
    {
        // Arrange
        var expectedCustomer = new Customer(_validCpf);

        _repository.Setup(r => r.CreateAsync(It.Is<Customer>(c => c.Cpf.Equals(_validCpf))))
            .ReturnsAsync(expectedCustomer);

        // Act
        var result = await _useCase.Execute(_validCpf, null, null);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedCustomer, options => options.ComparingByMembers<Customer>());
            result.Id.Should().NotBeEmpty();
        }
    }

    [Theory]
    [InlineData("310.686.640-37", "customer", "email@email.com")]
    public async Task Create_Customer_NameEmail_Success(string cpf, string name, string email)
    {
        // Arrange
        var customer = new Customer(cpf, name, email);
        _repository.Setup(r => r.CreateAsync(It.Is<Customer>(c => c.Cpf.Equals(customer.Cpf))))
            .ReturnsAsync(customer);

        // Act
        var result = await _useCase.Execute(cpf, name, email);

        // Assert
        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().Be(customer);
        }
    }

    [Fact]
    public async Task Create_Duplicated_Cpf_Customer_Error()
    {
        // Arrange
        _repository.Setup(r => r.FindByCpfAsync(It.IsAny<string>()))
            .ReturnsAsync(new Customer(_validCpf));

        // Act
        var func = async () => await _useCase.Execute(_validCpf, null, null);

        // Assert
        using (new AssertionScope())
        {
            (await func.Should().ThrowExactlyAsync<UseCaseException>())
                .And
                .Message
                .Should()
                .Be("There's already a customer using the provided CPF value.");

            _repository.Verify(r => r.CreateAsync(It.IsAny<Customer>()), Times.Never);
        }
    }
}
