using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs.CommonDTOs;
using TodoApi.DTOs.RequestDTOs;
using TodoApi.DTOs.ResponseDTOs;
using TodoApi.Interfaces;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {

        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTodoAsync([FromBody] CreateTodo todo)
        {
            var result = await _todoService.CreateTodoAsync(todo);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTodoAsync(int id)
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            return todo == null ?
                throw new KeyNotFoundException($"Todo with id:{id} not found.")
                : Ok(todo);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Todo>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTodosAsync()
        {
            var todos = await _todoService.GetAllTodosAsync();
            return Ok(todos);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTodoAsync(int id, [FromBody] UpdateTodo request)
        {
            var existingTodo = await _todoService.GetTodoByIdAsync(request.Id);
            if (existingTodo == null)
            {
                throw new KeyNotFoundException($"Todo with id:{id} not found.");
            }

            var result = _todoService.UpdateTodoAsync(request.Id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTodoAsync(int id)
        {
            var result = await _todoService.DeleteTodoAsync(id);
            if (!result)
            {
                throw new KeyNotFoundException($"Todo with id:{id} not found.");
            }

            return Ok(new SimpleResponse()
            {
                StatusCode = StatusCodes.Status200OK,
                Message = "Todo deleted successfully",
                Success = true
            });

        }
    }
}
