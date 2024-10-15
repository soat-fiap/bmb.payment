using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.Interfaces;
using Bmb.Payment.MySql.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;

namespace Bmb.Payment.MySql;

[ExcludeFromCodeCoverage]
public static class ServiceExtensions
{
    public static void ConfigurePersistenceApp(this IServiceCollection services, IConfiguration configuration)
    {
        if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("MySql")))
        {
            services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
        }
        else
        {
            services.AddScoped<IDbConnection>(_ =>
            {
                DbProviderFactories.RegisterFactory("MySql.Data.MySqlClient", MySqlClientFactory.Instance);
                var providerFactory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
                var conn = providerFactory.CreateConnection();
                conn.ConnectionString = configuration.GetConnectionString("MySql");
                conn.Open();
                return conn;
            });

            services.AddScoped<IPaymentRepository, PaymentRepositoryDapper>();
        }
    }
}
