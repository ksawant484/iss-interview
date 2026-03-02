using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Controllers;
using TodoApi.DTOs.CommonDTOs;
using TodoApi.DTOs.RequestDTOs;
using TodoApi.DTOs.ResponseDTOs;
using TodoApi.Interfaces;

namespace TodoApi.Tests.Controllers
{
    public class TodoControllerTests
    {
        private readonly Mock<ITodoService> _mockService;
        private readonly Mock<ILogger<TodoController>> _mockLogger;
        private readonly TodoController _controller;

        public TodoControllerTests()
        {
            _mockService = new Mock<ITodoService>();
            _mockLogger = new Mock<ILogger<TodoController>>();

            _controller = new TodoController(
                _mockService.Object,
                _mockLogger.Object);
        }

        [Fact]
        public async Task CreateTodoAsync_ShouldReturnOk_WithCreatedTodo()
        {
            var request = new CreateTodo
            {
                Title = "Test Todo",
                Description = "Test Description",
                IsCompleted = false
            };

            var createdTodo = new Todo
            {
                Id = 1,
                Title = request.Title,
                Description = request.Description,
                IsCompleted = request.IsCompleted,
                CreatedAt = DateTime.UtcNow
            };

            _mockService
                .Setup(x => x.CreateTodoAsync(request))
                .ReturnsAsync(createdTodo);

            var result = await _controller.CreateTodoAsync(request);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

            okResult.Value.Should().BeEquivalentTo(createdTodo);

            _mockService.Verify(x => x.CreateTodoAsync(request), Times.Once);
        }

        [Fact]
        public async Task GetTodoAsync_ShouldReturnOk_WhenTodoExists()
        {

            var todo = new Todo
            {
                Id = 1,
                Title = "Test"
            };

            _mockService
                .Setup(x => x.GetTodoByIdAsync(1))
                .ReturnsAsync(todo);

            var result = await _controller.GetTodoAsync(1);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

            okResult.Value.Should().BeEquivalentTo(todo);

            _mockService.Verify(x => x.GetTodoByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetTodoAsync_ShouldThrowKeyNotFoundException_WhenNotFound()
        {
            _mockService
                .Setup(x => x.GetTodoByIdAsync(1))
                .ReturnsAsync((Todo?)null);

            Func<Task> act = async () => await _controller.GetTodoAsync(1);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Todo with id:1 not found.");
        }

        [Fact]
        public async Task GetAllTodosAsync_Should_Return_Ok_With_Todos()
        {
            var todos = new List<Todo>
        {
            new Todo { Id = 1, Title = "Todo1" },
            new Todo { Id = 2, Title = "Todo2" }
        };

            _mockService
                .Setup(x => x.GetAllTodosAsync())
                .ReturnsAsync(todos);

            var result = await _controller.GetAllTodosAsync();

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

            okResult.Value.Should().BeEquivalentTo(todos);

            _mockService.Verify(x => x.GetAllTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateTodoAsync_ShouldReturnOk_WhenUpdateSucceeds()
        {
            var request = new UpdateTodo
            {
                Title = "Updated Title",
                Description = "Updated Desc",
                IsCompleted = true
            };

            var updatedTodo = new Todo
            {
                Id = 1,
                Title = request.Title,
                Description = request.Description,
                IsCompleted = true
            };

            _mockService
                .Setup(x => x.UpdateTodoAsync(1, request))
                .ReturnsAsync(updatedTodo);

            var result = await _controller.UpdateTodoAsync(1, request);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

            okResult.Value.Should().BeEquivalentTo(updatedTodo);

            _mockService.Verify(x => x.UpdateTodoAsync(1, request), Times.Once);
        }

        [Fact]
        public async Task UpdateTodoAsync_ShouldThrowKeyNotFoundException_WhenNotFound()
        {
            var request = new UpdateTodo();

            _mockService
                .Setup(x => x.UpdateTodoAsync(1, request))
                .ReturnsAsync((Todo?)null);

            Func<Task> act = async () => await _controller.UpdateTodoAsync(1, request);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Todo with id:1 not found.");
        }

        [Fact]
        public async Task DeleteTodoAsync_ShouldReturnOk_WhenDeleteSucceeds()
        {
            _mockService
                .Setup(x => x.DeleteTodoAsync(1))
                .ReturnsAsync(true);

            var result = await _controller.DeleteTodoAsync(1);

            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

            var response = okResult.Value.Should()
                .BeOfType<SimpleResponse>().Subject;

            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(200);
            response.Message.Should().Be("Todo deleted successfully");

            _mockService.Verify(x => x.DeleteTodoAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteTodoAsync_ShouldThrowKeyNotFoundException_WhenNotFound()
        {
            _mockService
                .Setup(x => x.DeleteTodoAsync(1))
                .ReturnsAsync(false);

            Func<Task> act = async () => await _controller.DeleteTodoAsync(1);

            await act.Should().ThrowAsync<KeyNotFoundException>()
                .WithMessage("Todo with id:1 not found.");
        }
    }

}