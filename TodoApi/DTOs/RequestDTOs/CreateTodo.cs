namespace TodoApi.DTOs.RequestDTOs
{
    public class CreateTodo
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
    }
}
