namespace TodoApi.DTOs.ResponseDTOs
{
    public class ProblemResponse : SimpleResponse
    {
        public new bool Success = false;
        public string Title { get; set; }
        public string Instance { get; set; }
        public string Method { get; set; }
    }
}
