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
            try
            {
                var result = await _todoService.CreateTodoAsync(todo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTodoAsync(int id)
        {
            try
            {
                var todo = await _todoService.GetTodoByIdAsync(id);
                if (todo == null)
                {
                    return NotFound(new SimpleResponse()
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"Todo with id:{id} not found.",
                        Success = false
                    });
                }
                return Ok(todo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Todo>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTodosAsync()
        {
            try
            {
                var todos = await _todoService.GetAllTodosAsync();
                return Ok(todos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(Todo), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTodoAsync(int id, [FromBody] UpdateTodo request)
        {
            try
            {
                var existingTodo = await _todoService.GetTodoByIdAsync(request.Id);
                if (existingTodo == null)
                {
                    return NotFound(new SimpleResponse()
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"Todo with id:{id} not found.",
                        Success = false
                    });
                }

                var result = _todoService.UpdateTodoAsync(request.Id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(SimpleResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTodoAsync(int id)
        {
            try
            {
                var result = await _todoService.DeleteTodoAsync(id);
                if (result)
                {
                    return Ok(new SimpleResponse()
                    {
                        StatusCode = StatusCodes.Status200OK,
                        Message = "Todo deleted successfully",
                        Success = true
                    });
                }
                return NotFound(new SimpleResponse()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = $"Todo with id:{id} not found.",
                    Success = false
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
