using System.Diagnostics.CodeAnalysis;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Bmb.Payment.DI.HealthChecks;

[ExcludeFromCodeCoverage]
public class DynamoDbTableHealthCheck(IAmazonDynamoDB dynamoDb, string tableName) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await dynamoDb.DescribeTableAsync(new DescribeTableRequest
            {
                TableName = tableName
            }, cancellationToken);

            if (response.Table.TableStatus == TableStatus.ACTIVE)
            {
                return HealthCheckResult.Healthy();
            }

            return HealthCheckResult.Unhealthy($"Table {tableName} is not active.");
        }
        catch (ResourceNotFoundException)
        {
            return HealthCheckResult.Unhealthy($"Table {tableName} does not exist.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                $"An error occurred while checking the table {tableName}: {ex.Message}");
        }
    }
}

public static class DynamoDbHealthCheckExtensions
{
    public static IHealthChecksBuilder AddDynamoDbHealthCheck(this IHealthChecksBuilder builder, string tableName,
        HealthStatus? failureStatus = default, IEnumerable<string> tags = default,
        TimeSpan? timeout = default)
    {
        return builder.Add(new HealthCheckRegistration(
            $"{tableName}-health",
            sp => new DynamoDbTableHealthCheck(sp.GetRequiredService<IAmazonDynamoDB>(), tableName),
            failureStatus,
            tags,
            timeout));
    }
}