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
        private readonly ILogger<TodoController> _logger;

        public TodoController(ITodoService todoService, ILogger<TodoController> logger)
        {
            _todoService = todoService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateTodoAsync([FromBody] CreateTodo todo)
        {
            _logger.LogInformation("API invoked to create a new todo having title:{Title}", todo.Title);

            var result = await _todoService.CreateTodoAsync(todo);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTodoAsync(int id)
        {
            _logger.LogInformation("API invoked to fetch the todo having id:{Id}", id);

            var todo = await _todoService.GetTodoByIdAsync(id);
            return todo == null ?
                throw new KeyNotFoundException($"Todo with id:{id} not found.")
                : Ok(todo);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Todo>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTodosAsync()
        {
            _logger.LogInformation("API invoked to fetch all todos");

            var todos = await _todoService.GetAllTodosAsync();
            return Ok(todos);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTodoAsync(int id, [FromBody] UpdateTodo request)
        {
            _logger.LogInformation("API invoked to update the todo having id:{Id}", id);

            var result = await _todoService.UpdateTodoAsync(id, request);
            if (result == null)
            {
                throw new KeyNotFoundException($"Todo with id:{id} not found.");
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTodoAsync(int id)
        {
            _logger.LogInformation("API invoked to delete the todo having id:{Id}", id);

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
