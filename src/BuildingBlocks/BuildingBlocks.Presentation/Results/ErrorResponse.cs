namespace BuildingBlocks.Presentation.Results
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = null!;
        public Dictionary<string, List<string>>? Errors { get; set; }
    }
}