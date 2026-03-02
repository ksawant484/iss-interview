namespace TodoApi.DTOs.ResponseDTOs
{
    public class SimpleResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
