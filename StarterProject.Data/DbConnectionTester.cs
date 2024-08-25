namespace StarterProject.Data;

public static class DbConnectionTester
{
    public static void Test(string connectionString, string? provider)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException("Connection string not set.");
        }
        
        var connection = DbConnectionFactory.CreateConnection(connectionString, provider);
        connection.Open();
    }
}
