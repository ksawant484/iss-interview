using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using TodoApi.DTOs.CommonDTOs;
using TodoApi.Repository;

namespace TodoApi.Tests.Repository
{
    public class TodoRepositoryTests : IDisposable
    {
        private readonly string _connectionString;
        private readonly TodoRepository _repository;

        public TodoRepositoryTests()
        {
            // Use test database
            _connectionString = "Data Source=test_todos.db";

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                { "ConnectionStrings:SQLiteConnection", _connectionString }
                })
                .Build();

            _repository = new TodoRepository(config);

            CreateTable();
            ClearTable();
        }

        private void CreateTable()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            @"
                CREATE TABLE IF NOT EXISTS Todos (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Description TEXT NULL,
                    IsCompleted INTEGER NOT NULL,
                    CreatedAt TEXT NOT NULL
                );
            ";

            command.ExecuteNonQuery();
        }

        private void ClearTable()
        {
            using var connection = new SqliteConnection(_connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM Todos;";
            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            ClearTable();
        }

        [Fact]
        public async Task AddAsync_ReturnsTheSameTodoHavingId()
        {
            var todo = new Todo
            {
                Title = "Test Todo",
                Description = "Test Description",
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _repository.AddAsync(todo);

            result.Should().NotBeNull();
            result.Id.Should().BeGreaterThan(0);
            result.Title.Should().Be(todo.Title);
            result.Description.Should().Be(todo.Description);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsTodo_HavingExistingId()
        {
            var todo = await _repository.AddAsync(new Todo
            {
                Title = "GetById Test",
                Description = "Test Description for GetById Test",
                IsCompleted = false,
            });

            var result = await _repository.GetByIdAsync(todo.Id);

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(todo);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsNull_HavingNonExistentId()
        {
            var result = await _repository.GetByIdAsync(999);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEnumerableOfTodos()
        {
            var todo1 = await _repository.AddAsync(new Todo
            {
                Title = "Todo 1",
                CreatedAt = DateTime.UtcNow
            });

            var todo2 = await _repository.AddAsync(new Todo
            {
                Title = "Todo 2",
                CreatedAt = DateTime.UtcNow
            });

            IEnumerable<Todo> expected = [todo1, todo2];

            var result = await _repository.GetAllAsync();

            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expected, options => options
            .Using<DateTime>(ctx => ctx.Subject.ToUniversalTime()
                .Should().Be(ctx.Expectation.ToUniversalTime()))
            .WhenTypeIs<DateTime>());
        }

        [Fact]
        public async Task ExistsAsync_ReturnsTrue_HavingExistingId()
        {
            var todo = await _repository.AddAsync(new Todo
            {
                Title = "Exists Test",
                CreatedAt = DateTime.UtcNow
            });

            var exists = await _repository.ExistsAsync(todo.Id);

            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsAsync_ReturnsFalse_HavingNonExistentId()
        {
            var exists = await _repository.ExistsAsync(999);

            exists.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateAsync_ReturnsTodo_HavingCorrectData()
        {
            var todo = await _repository.AddAsync(new Todo
            {
                Title = "Old Title",
                CreatedAt = DateTime.UtcNow
            });

            todo.Title = "Updated Title";
            todo.IsCompleted = true;

            var updated = await _repository.UpdateAsync(todo);

            var result = await _repository.GetByIdAsync(todo.Id);

            result.Should().NotBeNull();
            result!.Title.Should().Be("Updated Title");
            result.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteTodo()
        {
            var todo = await _repository.AddAsync(new Todo
            {
                Title = "Delete Test",
                CreatedAt = DateTime.UtcNow
            });

            var deleted = await _repository.DeleteAsync(todo.Id);

            deleted.Should().BeTrue();

            var exists = await _repository.ExistsAsync(todo.Id);
            exists.Should().BeFalse();
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_HavingNonExistentId()
        {
            var result = await _repository.DeleteAsync(999);

            result.Should().BeFalse();
        }
    }

}
