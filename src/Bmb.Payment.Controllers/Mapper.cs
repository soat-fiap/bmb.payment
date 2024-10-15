using Bmb.Domain.Core.Entities;
using Bmb.Payment.Controllers.Dto;

namespace Bmb.Payment.Controllers;

internal static class Mapper
{
    internal static Customer ToDomain(this CustomerDto? customer)
    {
        return customer is null ? null : new Customer(customer.Id, customer.Cpf, customer.Name!, customer.Email!);
    }
}
