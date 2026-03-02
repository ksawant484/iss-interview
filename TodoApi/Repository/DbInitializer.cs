using Microsoft.Data.Sqlite;
using Serilog;

namespace TodoApi.Repository
{
    public static class DbInitializer
    {
        public static async Task InitializeDatabaseAsync(this WebApplication app, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=todos.db";
            using var connection = new SqliteConnection(connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Todos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT,
                    IsCompleted INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL
                )
            ";
            await command.ExecuteNonQueryAsync();

            Log.Information("Database initialized successfully");
        }
    }
}
