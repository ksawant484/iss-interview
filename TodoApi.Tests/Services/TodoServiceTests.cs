using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.DTOs.CommonDTOs;
using TodoApi.DTOs.RequestDTOs;
using TodoApi.Interfaces;
using TodoApi.Services;

namespace TodoApi.Tests.Services
{
    public class TodoServiceTests
    {
        private readonly Mock<IRepository<Todo>> _mockTodoRepo;
        private readonly Mock<ILogger<TodoService>> _mockLogger;
        private readonly ITodoService _todoService;

        public TodoServiceTests()
        {
            _mockTodoRepo = new Mock<IRepository<Todo>>();
            _mockLogger = new Mock<ILogger<TodoService>>();
            _todoService = new TodoService(_mockTodoRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateTodoAsync_ReturnsCreatedTodo_HavingCorrectData()
        {
            var createTodo = new CreateTodo { Title = "Test todo", Description = "Test description" };
            var createdTodo = new Todo { Id = 1, Title = "Test todo", Description = "Test description", CreatedAt = DateTime.UtcNow };

            _mockTodoRepo.Setup(o => o.AddAsync(It.IsAny<Todo>())).ReturnsAsync(createdTodo);

            var result = await _todoService.CreateTodoAsync(createTodo);

            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Title.Should().Be(createTodo.Title);
            result.Description.Should().Be(createTodo.Description);
        }

        [Fact]
        public async Task GetTodoByIdAsync_ReturnsTodo_HavingExistingId()
        {
            int todoId = 1;
            var existingTodo = new Todo { Id = todoId, Title = "Test Todo", Description = "Test description", CreatedAt = DateTime.UtcNow };

            _mockTodoRepo.Setup(o => o.GetByIdAsync(todoId)).ReturnsAsync(existingTodo);

            var result = await _todoService.GetTodoByIdAsync(todoId);

            result.Should().NotBeNull();
            result.Id.Should().Be(todoId);
            result.Title.Should().Be(existingTodo.Title);
            result.Description.Should().Be(existingTodo.Description);
        }

        [Fact]
        public async Task GetTodoByIdAsync_ReturnsNull_HavingNonExistentId()
        {
            int todoId = 999;
            _mockTodoRepo.Setup(o => o.GetByIdAsync(todoId)).ReturnsAsync((Todo?)null);

            var result = await _todoService.GetTodoByIdAsync(todoId);

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllTodosAsync_ReturnsEnumerableOfTodos()
        {
            IEnumerable<Todo> todoList = [
                new Todo { Id = 1, Title = "Test todo 1", Description = "Test description 1", CreatedAt = DateTime.UtcNow },
                new Todo { Id = 2, Title = "Test todo 2", Description = "Test description 2", CreatedAt = DateTime.UtcNow }
            ];

            _mockTodoRepo.Setup(o => o.GetAllAsync()).ReturnsAsync(todoList);

            var result = await _todoService.GetAllTodosAsync();

            result.Should().NotBeNullOrEmpty();
            result.Count().Should().Be(todoList.Count());
            result.Should().BeEquivalentTo(todoList);
        }

        [Fact]
        public async Task UpdateTodoAsync_ReturnsTodo_HavingCorrectData()
        {
            int id = 1;
            var updateTodo = new UpdateTodo { Id = id, Title = "Test todo completed", Description = "Test description marked complete", IsCompleted = true };
            var existingTodo = new Todo { Id = id, Title = "Test Todo", Description = "Test description", CreatedAt = DateTime.UtcNow };
            var updatedTodo = new Todo { Id = id, Title = "Test todo completed", Description = "Test description marked complete", IsCompleted = true, CreatedAt = DateTime.UtcNow };

            _mockTodoRepo.Setup(o => o.GetByIdAsync(id)).ReturnsAsync(existingTodo);
            _mockTodoRepo.Setup(o => o.UpdateAsync(It.IsAny<Todo>())).ReturnsAsync(updatedTodo);

            var result = await _todoService.UpdateTodoAsync(id, updateTodo);

            result.Should().NotBeNull();
            result.Id.Should().Be(existingTodo.Id);
            result.Title.Should().Be(updateTodo.Title);
            result.Description.Should().Be(updateTodo.Description);
            result.IsCompleted.Should().Be(updateTodo.IsCompleted);
        }

        [Fact]
        public async Task UpdateTodoAsync_ReturnsNull_HavingNonExistentId()
        {
            int id = 999;
            _mockTodoRepo.Setup(o => o.GetByIdAsync(id)).ReturnsAsync((Todo?)null);

            var result = await _todoService.UpdateTodoAsync(id, It.IsAny<UpdateTodo>());

            result.Should().BeNull();
        }

        [Fact]
        public async Task DeleteTodoAsync_ReturnsTrue_HavingExistingId()
        {
            int todoId = 1;
            _mockTodoRepo.Setup(o => o.ExistsAsync(todoId)).ReturnsAsync(true);
            _mockTodoRepo.Setup(r => r.DeleteAsync(todoId)).ReturnsAsync(true);

            var result = await _todoService.DeleteTodoAsync(todoId);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteTodoAsync_ReturnsFalse_HavingNonExistentId()
        {
            int todoId = 999;
            _mockTodoRepo.Setup(o => o.ExistsAsync(todoId)).ReturnsAsync(false);

            var result = await _todoService.DeleteTodoAsync(todoId);

            result.Should().BeFalse();
        }
    }
}
