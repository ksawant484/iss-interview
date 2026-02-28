using TodoApi.DTOs.CommonDTOs;
using TodoApi.DTOs.RequestDTOs;

namespace TodoApi.Interfaces
{
    public interface ITodoService
    {
        Task<Todo> CreateTodoAsync(CreateTodo createTodo);
        
        Task<Todo?> GetTodoByIdAsync(int id);

        Task<IEnumerable<Todo>> GetAllTodosAsync();

        Task<Todo> UpdateTodoAsync(int id, UpdateTodo updateTodo);

        Task<bool> DeleteTodoAsync(int id);

    }
}
