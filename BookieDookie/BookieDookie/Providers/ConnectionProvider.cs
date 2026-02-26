using System.Data;
using Npgsql;

namespace BookieDookie.Providers
{
    public static class ConnectionProvider
    {
        private static string? _connectionString;

        public static string? GetConnectionString(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            return _connectionString;
        }

        public static IDbConnection GetConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException(
                    "DapperConnectionProvider is not initialized. Call Initialize() first.");

            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}