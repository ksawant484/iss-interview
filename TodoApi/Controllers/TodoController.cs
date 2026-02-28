using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs.CommonDTOs;
using TodoApi.DTOs.RequestDTOs;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        public TodoController()
        {
        }

        [HttpPost]
        public IActionResult CreateTodo([FromBody] Todo todo)
        {
            try
            {
                var todoService = new TodoService();
                var result = todoService.CreateTodo(todo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetTodo(int id)
        {
            try
            {
                var todoService = new TodoService();
                var todo = todoService.GetTodoById(id);
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
        public IActionResult GetAllTodos()
        {
            try
            {
                var todoService = new TodoService();
                var todos = todoService.GetAllTodos();
                return Ok(todos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTodo(int id, [FromBody] UpdateTodo request)
        {
            try
            {
                var todoService = new TodoService();
                var existingTodo = todoService.GetTodoById(request.Id);
                if (existingTodo == null)
                {
                    return NotFound();
                }

                var todo = new Todo
                {
                    Title = request.Title,
                    Description = request.Description,
                    IsCompleted = request.IsCompleted
                };

                var result = todoService.UpdateTodo(request.Id, todo);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTodo(int id)
        {
            try
            {
                var todoService = new TodoService();
                var result = todoService.DeleteTodo(id);
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
