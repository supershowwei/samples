namespace SampleMVC.Protocol.Model.Results
{
    public interface IResult
    {
        bool IsSuccess { get; set; }

        bool IsFailure { get; set; }

        string Message { get; set; }
    }
}