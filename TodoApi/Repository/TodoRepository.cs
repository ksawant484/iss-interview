using Microsoft.Data.Sqlite;
using TodoApi.DTOs.CommonDTOs;
using TodoApi.Interfaces;

namespace TodoApi.Repository
{
    public class TodoRepository : IRepository<Todo>
    {
        private readonly string _dbConnString;

        public TodoRepository(IConfiguration configuration)
        {
            _dbConnString = configuration.GetConnectionString("SQLiteConnection")
                ?? "Data Source=todos.db";
        }

        public async Task<Todo> AddAsync(Todo resource)
        {
            using var connection = new SqliteConnection(_dbConnString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = $@"
                    INSERT INTO Todos (Title, Description, IsCompleted, CreatedAt)
                    VALUES (@title, @description, @isCompleted, @createdAt);
                    SELECT last_insert_rowid();
                ";

            command.Parameters.AddWithValue("@title", resource.Title);
            command.Parameters.AddWithValue("@description", resource.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@isCompleted", resource.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@createdAt", resource.CreatedAt.ToString("o"));

            var id = Convert.ToInt32(await command.ExecuteScalarAsync());
            resource.Id = id;
            return resource;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var connection = new SqliteConnection(_dbConnString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = $"DELETE FROM Todos WHERE Id = @id";

            command.Parameters.AddWithValue("@id", id);

            var rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            using var connection = new SqliteConnection(_dbConnString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(1) FROM Todos WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }

        public async Task<IEnumerable<Todo>> GetAllAsync()
        {
            var todos = new List<Todo>();
            using var connection = new SqliteConnection(_dbConnString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos ORDER BY CreatedAt DESC";

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                todos.Add(MapTodoFromReader(reader));
            }

            return todos;
        }

        public async Task<Todo?> GetByIdAsync(int id)
        {
            using var connection = new SqliteConnection(_dbConnString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Title, Description, IsCompleted, CreatedAt FROM Todos WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapTodoFromReader(reader);
            }

            return null;
        }

        public async Task<Todo> UpdateAsync(Todo resource)
        {
            using var connection = new SqliteConnection(_dbConnString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                    UPDATE Todos
                    SET Title = @title, Description = @description, IsCompleted = @isCompleted
                    WHERE Id = @id
                ";

            command.Parameters.AddWithValue("@title", resource.Title);
            command.Parameters.AddWithValue("@description", resource.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@isCompleted", resource.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@id", resource.Id);

            await command.ExecuteNonQueryAsync();
            return resource;
        }

        private Todo MapTodoFromReader(SqliteDataReader reader)
        {
            var descriptionOrdinal = reader.GetOrdinal("Description");
            return new Todo
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Description = reader.IsDBNull(descriptionOrdinal) ? null : reader.GetString(descriptionOrdinal),
                IsCompleted = reader.GetInt32(reader.GetOrdinal("IsCompleted")) == 1,
                CreatedAt = DateTime.Parse(reader.GetString(reader.GetOrdinal("CreatedAt")))
            };
        }
    }
}
