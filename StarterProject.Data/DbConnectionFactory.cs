using System.Data.Common;

namespace StarterProject.Data;

internal static class DbConnectionFactory
{
    private static string? _connectionString;
    private static string? _providerName;

    public static void SetConnectionString(string connectionString, string providerName)
    {
        _connectionString = connectionString;
        _providerName = providerName;
    }

    public static Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
    {
        return GetOpenDbConnectionAsync(_connectionString, _providerName, cancellationToken);
    }

    public static DbConnection GetConnection()
    {
        return GetOpenDbConnection(_connectionString, _providerName);
    }

    private static async Task<DbConnection> GetOpenDbConnectionAsync(string? connectionString, string? providerName, CancellationToken cancellationToken)
    {
        var connection = CreateConnection(connectionString, providerName);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

        return connection;
    }

    private static DbConnection GetOpenDbConnection(string? connectionString, string? providerName)
    {
        var connection = CreateConnection(connectionString, providerName);

        connection.Open();

        return connection;
    }

    internal static DbConnection CreateConnection(string? connectionString, string? providerName)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Connection string not set make sure to set it e.g DbConnectionFactory.SetConnectionString", nameof(connectionString));
        }

        var provider = DbProviderFactories.GetFactory(providerName ?? "System.Data.SqlClient");
        var connection = provider.CreateConnection() ?? throw new InvalidOperationException("Failed to create a database connection.");
        connection.ConnectionString = connectionString;

        return connection;
    }
}
