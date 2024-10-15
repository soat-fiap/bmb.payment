using System.Data;
using Dapper;
using Bmb.Domain.Core.Base;
using Bmb.Domain.Core.Entities;
using Bmb.Domain.Core.Interfaces;
using Bmb.Domain.Core.ValueObjects;
using Bmb.Payment.MySql.Dto;
using Microsoft.Extensions.Logging;

namespace Bmb.Payment.MySql.Repository;

public class PaymentRepositoryDapper(
    IDbConnection dbConnection,
    ILogger<PaymentRepositoryDapper> logger)
    : IPaymentRepository
{

    public async Task<Domain.Core.Entities.Payment> SaveAsync(Domain.Core.Entities.Payment payment)
    {
        logger.LogInformation("Persisting Payment {PaymentId} for Order {OrderId}", payment.Id.Value,
            payment.OrderId);

        var transaction = dbConnection.BeginTransaction();
        {
            try
            {
                var paymentDao = new PaymentDto(payment.Id.Value, payment.OrderId, (int)payment.Status,
                    (int)payment.PaymentType, payment.Amount, payment.ExternalReference, payment.Created, null);
                await dbConnection.ExecuteAsync(Constants.InsertPaymentQuery, paymentDao);

                transaction.Commit();

                logger.LogInformation("Payment {PaymentId} persisted", payment.Id.Value);
                return payment;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error Persisting Payment {PaymentId} for Order {OrderId}", payment.Id.Value,
                    payment.OrderId);
                transaction.Rollback();
                throw;
            }
        }
    }

    public async Task<Domain.Core.Entities.Payment?> GetPaymentAsync(PaymentId paymentId)
    {
        logger.LogInformation("Getting Payment {PaymentId}", paymentId.Value);

        var paymentDao = await dbConnection.QuerySingleOrDefaultAsync<PaymentDto>(
            Constants.GetPaymentQuery,
            param: new { Id = paymentId.Value }
        );

        if (paymentDao is null)
        {
            return null;
        }

        logger.LogInformation("Payment {PaymentId} retrieved", paymentId.Value);
        return new Domain.Core.Entities.Payment()
        {
            Id = new PaymentId(paymentDao.Id),
            OrderId = paymentDao.OrderId,
            PaymentType = (PaymentType)paymentDao.PaymentType,
            QrCode = paymentDao.QrCode,
            Amount = paymentDao.Amount,
            Created = paymentDao.Created,
            Status = (PaymentStatus)paymentDao.Status
        };
    }

    public async Task<Domain.Core.Entities.Payment?> GetPaymentAsync(string externalReference, PaymentType paymentType)
    {
        logger.LogInformation("Getting {PaymentType} Payment by ExternalReference {ExternalReference}",
            paymentType.ToString(), externalReference);

        var paymentDao = await dbConnection.QuerySingleOrDefaultAsync<PaymentDto>(
            Constants.GetPaymentByExternalReferenceQuery,
            param: new
            {
                ExternalReference = externalReference,
                PaymentType = paymentType
            }
        );

        if (paymentDao is null)
        {
            return null;
        }

        logger.LogInformation("{PaymentType} Payment {PaymentId} retrieved", paymentType.ToString(), paymentDao.Id);
        return new Domain.Core.Entities.Payment()
        {
            Id = new PaymentId(paymentDao.Id),
            OrderId = paymentDao.OrderId,
            PaymentType = (PaymentType)paymentDao.PaymentType,
            QrCode = paymentDao.QrCode,
            Amount = paymentDao.Amount,
            Created = paymentDao.Created,
            Status = (PaymentStatus)paymentDao.Status,
            ExternalReference = paymentDao.ExternalReference
        };
    }

    public async Task<bool> UpdatePaymentStatusAsync(Domain.Core.Entities.Payment payment)
    {
        logger.LogInformation("Updating Payment {PaymentId} status", payment.Id.Value);

        var updated = await dbConnection.ExecuteAsync(
            Constants.UpdatePaymentStatusQuery,
            new
            {
                Id = payment.Id.Value,
                Status = (int)payment.Status,
                payment.Updated,
            }) == 1;

        logger.LogInformation(
            updated
                ? "Payment {PaymentId} status updated to {PaymentStatus}"
                : "Payment {PaymentId} status not updated to {PaymentStatus}", payment.Id.Value, payment.Status);

        return updated;
    }
}
