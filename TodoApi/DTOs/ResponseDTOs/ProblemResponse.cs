namespace TodoApi.DTOs.ResponseDTOs
{
    public class ProblemResponse : SimpleResponse
    {
        public ProblemResponse()
        {
            Message = "Something went wrong. Please check logs.";
            Success = false;
        }

        public string Title { get; set; }
        public string Instance { get; set; }
        public string Method { get; set; }
    }
}
