using Npgsql;
using System;

class TestConnection
{
    static async Task Main(string[] args)
    {
        // Test different connection string formats
        var connectionStrings = new[]
        {
            "Host=db.hvsdlufeqgtzflrbaoyr.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=Medical@2025;SSL Mode=Require;Trust Server Certificate=true",
            "Host=aws-0-us-east-1.pooler.supabase.com;Port=6543;Database=postgres;Username=postgres.hvsdlufeqgtzflrbaoyr;Password=Medical@2025;SSL Mode=Require;Trust Server Certificate=true",
            "postgresql://postgres:Medical%402025@db.hvsdlufeqgtzflrbaoyr.supabase.co:5432/postgres?sslmode=require"
        };

        foreach (var connString in connectionStrings)
        {
            Console.WriteLine($"\nTesting: {connString.Substring(0, Math.Min(50, connString.Length))}...");
            try
            {
                await using var connection = new NpgsqlConnection(connString);
                await connection.OpenAsync();
                Console.WriteLine("✓ Connection successful!");
                
                await using var command = new NpgsqlCommand("SELECT version()", connection);
                var result = await command.ExecuteScalarAsync();
                Console.WriteLine($"  PostgreSQL version: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Failed: {ex.Message}");
            }
        }
    }
}
