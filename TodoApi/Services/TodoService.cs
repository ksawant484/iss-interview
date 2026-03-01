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

        public TodoService(IRepository<Todo> todoRepository)
        {
            _todoRepository = todoRepository;
        }

        public async Task<Todo> CreateTodoAsync(CreateTodo createTodo)
        {
            try
            {
                Todo todo = new()
                {
                    Title = createTodo.Title,
                    Description = createTodo.Description,
                    IsCompleted = createTodo.IsCompleted,
                    CreatedAt = DateTime.UtcNow
                };

                Todo createdTodo = await _todoRepository.AddAsync(todo);

                return createdTodo;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Todo?> GetTodoByIdAsync(int id)
        {
            try
            {
                var todo = await _todoRepository.GetByIdAsync(id);
                return todo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Todo>> GetAllTodosAsync()
        {
            try
            {
                var todos = await _todoRepository.GetAllAsync();
                return todos;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Todo?> UpdateTodoAsync(int id, UpdateTodo updateTodo)
        {
            try
            {
                var existingTodo = await _todoRepository.GetByIdAsync(id);

                if (existingTodo == null)
                {
                    return null;
                }

                existingTodo.Title = updateTodo.Title;
                existingTodo.Description = updateTodo.Description;
                existingTodo.IsCompleted = existingTodo.IsCompleted;

                var updatedTodo = await _todoRepository.UpdateAsync(existingTodo);

                return updatedTodo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteTodoAsync(int id)
        {
            try
            {
                bool exists = await _todoRepository.ExistsAsync(id);
                
                if (!exists)
                {
                    return false;
                }

                bool result = await _todoRepository.DeleteAsync(id);

                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
