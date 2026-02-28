using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs.RequestDTOs;
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
        public async Task<IActionResult> GetTodoAsync(int id)
        {
            try
            {
                var todo = await _todoService.GetTodoByIdAsync(id);
                if (todo == null)
                {
                    return NotFound();
                }
                return Ok(todo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
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
        public async Task<IActionResult> UpdateTodoAsync(int id, [FromBody] UpdateTodo request)
        {
            try
            {
                var existingTodo = await _todoService.GetTodoByIdAsync(request.Id);
                if (existingTodo == null)
                {
                    return NotFound();
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
        public async Task<IActionResult> DeleteTodoAsync(int id)
        {
            try
            {
                var result = await _todoService.DeleteTodoAsync(id);
                if (result)
                {
                    return Ok(new { message = "Todo deleted successfully" });
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
