using System.Data;
using System.Diagnostics.CodeAnalysis;
using Bmb.Domain.Core.Base;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Bmb.Payment.MySql;

[ExcludeFromCodeCoverage]
public class MySqlDbContext : IDbContext, IDisposable
{
    private readonly IConfiguration _configuration;
    private IDbConnection? _dbConnection;

    public MySqlDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        if (_dbConnection is null)
        {
            var connection = _configuration.GetConnectionString("MySql");
            _dbConnection = new MySqlConnection(connection);
            _dbConnection.Open();
        }

        return _dbConnection;
    }

    public void Dispose()
    {
        _dbConnection?.Close();
    }
}
