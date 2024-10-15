using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.Entities;
using Bmb.Domain.Core.Interfaces;
using Bmb.Domain.Core.ValueObjects;

namespace Bmb.Payment.MySql.Repository;

[ExcludeFromCodeCoverage]
public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly List<Domain.Core.Entities.Payment> _payments = new();

    public Task<Domain.Core.Entities.Payment> SaveAsync(Domain.Core.Entities.Payment payment)
    {
        _payments.Add(payment);
        return Task.FromResult(payment);
    }

    public Task<Domain.Core.Entities.Payment?> GetPaymentAsync(PaymentId paymentId)
    {
        var payment = _payments.First(p => p.Id == paymentId) ?? default;
        return Task.FromResult(payment);
    }

    public Task<Domain.Core.Entities.Payment?> GetPaymentAsync(string externalReference, PaymentType paymentType)
    {
        var payment = _payments.First(p => p.ExternalReference == externalReference && p.PaymentType == paymentType);
        return Task.FromResult(payment ?? default);
    }

    public Task<bool> UpdatePaymentStatusAsync(Domain.Core.Entities.Payment payment)
    {
        var index = _payments.FindIndex(p => p.Id == payment.Id);
        if (index < 0)
        {
            return Task.FromResult(false);
        }

        _payments[index] = payment;
        return Task.FromResult(true);
    }
}
