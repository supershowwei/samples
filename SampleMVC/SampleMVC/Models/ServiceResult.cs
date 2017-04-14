namespace SampleMVC.Models
{
    public abstract class ServiceResult
    {
        public bool IsSuccess { get; set; }

        public bool IsFailure { get; set; }

        public string Message { get; set; }
    }
}