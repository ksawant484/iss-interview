using Microsoft.Data.Sqlite;
using TodoApi.DTOs.CommonDTOs;
using TodoApi.DTOs.RequestDTOs;
using TodoApi.Interfaces;
using TodoApi.Repository;

namespace TodoApi.Services
{
    public class TodoService : ITodoService
    {
        private readonly IRepository<Todo> _todoRepository;
        private readonly ILogger<TodoService> _logger;

        public TodoService(IRepository<Todo> todoRepository, ILogger<TodoService> logger)
        {
            _todoRepository = todoRepository;
            _logger = logger;
        }

        public async Task<Todo> CreateTodoAsync(CreateTodo createTodo)
        {
            _logger.LogInformation("Creating new todo with title:{Title}", createTodo.Title);

            Todo todo = new()
            {
                Title = createTodo.Title,
                Description = createTodo.Description,
                IsCompleted = createTodo.IsCompleted,
                CreatedAt = DateTime.UtcNow
            };

            Todo createdTodo = await _todoRepository.AddAsync(todo);

            _logger.LogInformation("New todo created successfully with id:{Id}", createdTodo.Id);

            return createdTodo;
        }

        public async Task<Todo?> GetTodoByIdAsync(int id)
        {
            _logger.LogInformation("Fetching todo with id:{Id}", id);

            var todo = await _todoRepository.GetByIdAsync(id);
            return todo;
        }

        public async Task<IEnumerable<Todo>> GetAllTodosAsync()
        {
            _logger.LogInformation("Fetching all todos");

            var todos = await _todoRepository.GetAllAsync();
            return todos;
        }

        public async Task<Todo?> UpdateTodoAsync(int id, UpdateTodo updateTodo)
        {
            _logger.LogInformation("Updating the todo having id:{Id}", id);

            var existingTodo = await _todoRepository.GetByIdAsync(id);

            if (existingTodo == null)
            {
                _logger.LogWarning("Todo having the id:{Id} is not available for update", id);
                return null;
            }

            existingTodo.Title = string.IsNullOrWhiteSpace(updateTodo.Title) ? existingTodo.Title : updateTodo.Title;
            existingTodo.Description = string.IsNullOrWhiteSpace(updateTodo.Description) ? existingTodo.Description : updateTodo.Description;
            existingTodo.IsCompleted = updateTodo.IsCompleted == null ? existingTodo.IsCompleted : updateTodo.IsCompleted;

            var updatedTodo = await _todoRepository.UpdateAsync(existingTodo);

            _logger.LogInformation("Updated the todo successfully having id:{Id}", id);

            return updatedTodo;
        }

        public async Task<bool> DeleteTodoAsync(int id)
        {
            _logger.LogInformation("Deleting the todo having id:{Id}", id);

            bool exists = await _todoRepository.ExistsAsync(id);

            if (!exists)
            {
                _logger.LogWarning("Todo having id:{Id} is not available for deletion", id);
                return false;
            }

            bool result = await _todoRepository.DeleteAsync(id);

            if (result)
            {
                _logger.LogInformation("Deleted the todo successfully having id:{Id}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete the todo having id:{Id}", id);
            }

            return result;
        }
    }
}
