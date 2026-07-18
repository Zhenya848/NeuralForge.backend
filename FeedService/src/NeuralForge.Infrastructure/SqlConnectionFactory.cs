using System.Data;
using Microsoft.Extensions.Configuration;
using NeuralForge.Application.Abstractions;
using Npgsql;

namespace NeuralForge.Infrastructure
{
    public class SqlConnectionFactory(IConfiguration configuration) : ISqlConnectionFactory
    {
        public IDbConnection Create() =>
            new NpgsqlConnection(configuration.GetConnectionString("Database"));
    }
}
